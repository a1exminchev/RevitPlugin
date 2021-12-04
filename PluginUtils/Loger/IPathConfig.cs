namespace PluginUtil.Loger
{
    public interface IIiPathConfig
    {
        #region class props

        string LogPath { get; set; }
        string ModelFolderName { get; }

        bool EnableLogging { get; set; }

        #endregion
    }
}