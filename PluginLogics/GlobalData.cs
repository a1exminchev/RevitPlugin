using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace PluginLogics
{
    public static class GlobalData
    {
        #region All props

        #region Props

        #region class props

        public static UIDocument uiDoc { get; set; }
        public static UIControlledApplication UiCApp { get; set; }
        public static Application app { get; set; }
        public static Document doc { get; set; }
        public static string Version { get; set; }



        public static UIApplication UIApplication { get; set; }


        public static Options Opt
        {
            get
            {
                if (Opt1 == null)
                {
                    Opt1 = new Options();
                    Opt1.ComputeReferences = true;
                }

                return Opt1;
            }
        }

        public static int VersionInt
        {
            get
            {
                if (_versionInt == 0)
                {
                    if (Version.Contains("2017"))
                        _versionInt = 2017;
                    else if (Version.Contains("2018"))
                        _versionInt = 2018;
                    else if (Version.Contains("2019"))
                        _versionInt = 2019;
                    else if (Version.Contains("2020"))
                        _versionInt = 2020;
                    else if (Version.Contains("2021"))
                        _versionInt = 2021;
                    else if (Version.Contains("2022")) _versionInt = 2022;
                }

                return _versionInt;
            }
            set => _versionInt = value;
        }

        public static ViewSection ActiveView { get; set; }

        #endregion

        #endregion

        #endregion

        #region static

        public static string TempView3DName = "TempView3D";

        private static Options Opt1;
        private static int _versionInt;

        public static void Init(UIDocument uidoc)
        {
            uiDoc = uidoc;
            doc = uidoc.Document;
            UIApplication = uidoc.Application;
            app = UIApplication.Application;
        }



        public static void Init(UIApplication uicApp, bool createTempView = false)
        {
            Version = uicApp.Application.VersionName;
            UIApplication = uicApp;
            app = uicApp.Application;
            uiDoc = uicApp?.ActiveUIDocument;
            doc = uiDoc?.Document;
        }

        #endregion
    }
}