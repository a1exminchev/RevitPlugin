namespace PluginUtil.Loger
{
    public class PathConfig : IIiPathConfig
    {
        #region Interface_Implementations

        public string LogPath
        {
            get
            {
                if (string.IsNullOrEmpty(_path)) _path = Log.LogDirectory;

                return _path;
            }
            set => _path = value;
        }

        public string ModelFolderName => "Models";
        public bool EnableLogging { get; set; } = true;

        #endregion

        #region fields

        private string _modelFolderName;
        private string _path;

        #endregion
    }
}