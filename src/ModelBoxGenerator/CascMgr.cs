using CASCLib;

namespace ModelBoxGenerator
{
    internal sealed class CascMgr
    {
        private static readonly Lazy<CascMgr> _instance = new(() => new CascMgr());
        public static CascMgr Instance => _instance.Value;
        private CascMgr() { }

        private CASCHandler? _handler;

        private bool _isLoaded = false;

        public void Initialize()
        {
            if (_isLoaded)
                return;

            _handler = CASCHandler.OpenOnlineStorage("wow");
            _handler.Root.SetFlags(LocaleFlags.enUS, createTree: true);

            _isLoaded = true;
        }

        public Stream? OpenFile(uint fileDataId)
        {
            if (!_isLoaded || _handler is null)
                throw new Exception("CASC must be initialized before opening file.");

            Stream? stream = null;

            try
            {
                stream = _handler.OpenFile((int)fileDataId);
            }
            catch (FileNotFoundException)
            {
                // file exists in listfile, but not in storage. Nothing can be done.
            }
            catch(BLTEDecoderException)
            {
                // file is encrypted, nothing can be done.
            }

            return stream;

        }

    }
}
