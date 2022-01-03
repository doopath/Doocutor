using System.ComponentModel.DataAnnotations;
using CUI;
using CUI.OutBuffers;

namespace InputHandling
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
        public virtual int UpdateRate { get; set; }

        protected readonly IOutBuffer _outBuffer;
        protected bool _isActive = true;

        public OutBufferSizeHandler(IOutBuffer outBuffer, int updateRate)
        {
            UpdateRate = updateRate;
            _outBuffer = outBuffer;
        }

        public virtual Task Start()
        {
            if (!_isActive)
                _isActive = true;

            return Task.Run(AdaptOutBufferSize);
        }

        public virtual void Stop()
            => _isActive = false;

        protected virtual void AdaptOutBufferSize()
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
                        CuiRender.DisableOutBufferCursor();
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
