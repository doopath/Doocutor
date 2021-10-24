using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Core.OutBuffers;
using Domain.Core.Exceptions;
using Libraries.Core;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;

namespace DynamicEditor.Core
{
    public class OutBufferSizeHandler
    {
        /// <summary>
        /// Update rate in milliseconds.
        /// Value cannot be negative and should be in a range [0; 1000].
        /// It's not recommended to set a value less than 100,
        /// because the behavior becomes unstable.
        /// </summary>
        [Range(0, 1000)]
        public int UpdateRate { get; set; }

        private readonly IOutBuffer _outBuffer;
        private readonly CuiRender _render;
        private bool _isActive = true;

        public OutBufferSizeHandler(IOutBuffer outBuffer, CuiRender render, int updateRate)
        {
            UpdateRate = updateRate;
            _outBuffer = outBuffer;
            _render = render;
        }

        [SupportedOSPlatform("windows")]
        public Task Start()
        {
            if (!OperatingSystem.IsWindows())
                throw new UnsupportedOSPlatformException(
                    $"Cannot run {nameof(OutBufferSizeHandler)} on {Environment.OSVersion}");
            
            if (!_isActive)
                _isActive = true;

            return Task.Run(AdaptOutBufferSize);
        }

        public void Stop()
            => _isActive = false;

        private void AdaptOutBufferSize()
        {
            while (_isActive)
            {
                var width = _outBuffer.Width;
                var height = _outBuffer.Height;

                Thread.Sleep(UpdateRate);

                try
                {
                    if (width != _outBuffer.Width || height != _outBuffer.Height)
                    {
                        _render.Clear();
                        _render.Render();
                    }
                }
                catch (ArgumentOutOfRangeException) {
                    _render.Render();
                }
                catch (Exception exc)
                {
                    ErrorHandling.fileLogger.Error(exc);
                }
            }
        }
    }
}