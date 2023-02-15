using CUI.OutBuffers;
using System.ComponentModel.DataAnnotations;

namespace CUI
{
    public sealed class OutBufferSizeHandler
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
        private bool _isActive = true;

        public OutBufferSizeHandler(IOutBuffer outBuffer, int updateRate)
        {
            UpdateRate = updateRate;
            _outBuffer = outBuffer;
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

                Thread.Sleep(UpdateRate);

                try
                {
                    if (width != _outBuffer.Width || height != _outBuffer.Height)
                    {
                        CuiRender.Clear();
                        CuiRender.Render();
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    CuiRender.Render();
                }
                catch (Exception exc)
                {
                    ErrorHandling.FileLogger.Error(exc);
                }
            }
        }
    }
}
