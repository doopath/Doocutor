using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Core.OutBuffers;

namespace DynamicEditor.Core
{
    internal class OutBufferSizeHandler
    {
        private readonly IOutBuffer _outBuffer;
        private readonly CuiRender _render;
        private readonly int _updateRate;
        private bool _isActive;

        public OutBufferSizeHandler(IOutBuffer outBuffer, CuiRender render, int updateRate)
        {
            _outBuffer = outBuffer;
            _render = render;
            _updateRate = updateRate;
            _isActive = true;
        }

        public void Start()
        {
            if (!_isActive)
                _isActive = true;

            Task.Run(AdaptOutBufferSize);
        }

        public void Stop()
            => _isActive = false;

        private void AdaptOutBufferSize()
        {
            while (_isActive)
            {
                var width = _outBuffer.Width;
                var height = _outBuffer.Height;

                Thread.Sleep(_updateRate);

                try
                {
                    if (width != _outBuffer.Width || height != _outBuffer.Height)
                        _render.Render();
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
    }
}