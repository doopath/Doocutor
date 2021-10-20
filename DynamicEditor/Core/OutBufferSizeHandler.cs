using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Core.OutBuffers;
using Libraries.Core;

namespace DynamicEditor.Core
{
    public class OutBufferSizeHandler
    {
        public int UpdateRate { get; set; }
        private readonly IOutBuffer _outBuffer;
        private readonly CuiRender _render;
        private bool _isActive;

        public OutBufferSizeHandler(IOutBuffer outBuffer, CuiRender render, int updateRate)
        {
            UpdateRate = updateRate;
            _outBuffer = outBuffer;
            _render = render;
            _isActive = true;
        }

        public Task Start()
        {
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
                        _render.Render();
                }
                catch (ArgumentOutOfRangeException) { }
                catch (Exception exc)
                {
                    ErrorHandling.fileLogger.Error(exc);
                }
            }
        }
    }
}