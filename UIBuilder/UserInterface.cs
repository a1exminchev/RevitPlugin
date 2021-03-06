using System;
using Autodesk.Revit.Attributes;
using ArCm;
using StudyTask;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;

namespace UIBuilder
{
	public class UserInterface
	{
		public static void CreateUserInterface(UIControlledApplication application, bool isRelease)
		{
            string folderPath = @"C:\Users\Aleksey Minchev\source\repos\RevitPlugin\RevitPlugin\bin\x64\Debug";
            if (isRelease == true)
            {
                folderPath = @"C:\Users\Aleksey Minchev\source\repos\RevitPlugin\RevitPlugin\bin\x64\Release";
            }
            string folderImage = @"C:\Users\Aleksey Minchev\source\repos\RevitPlugin\RevitPlugin\Images";

            string dllArCm = Path.Combine(folderPath, "ArCm.dll");
            string tabArCm = "АР";
            application.CreateRibbonTab(tabArCm);
            RibbonPanel panelTag = application.CreateRibbonPanel(tabArCm, "Tags");

            PushButton btnLayersTag = (PushButton)panelTag.AddItem(new PushButtonData("Layers tag", "Layers tag", dllArCm, typeof(LayerTag).FullName));
            btnLayersTag.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTag32.png"), UriKind.Absolute));
            btnLayersTag.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTag16.png"), UriKind.Absolute));
            btnLayersTag.ToolTip = "Clicking at Wall/Floor/Roof and select the side for annotation of all layers of construction";
            btnLayersTag.LongDescription = "Не забудь нажать единожды при открытии модели кнопку TagAutoRefreshing, чтобы аннотации автоматически обновлялись при изменениях конструкций";

            PushButton btnRegisterLayerUpdater = (PushButton)panelTag.AddItem(new PushButtonData("RegisterLayerUpdater", "TagAutoRefreshing", dllArCm, typeof(RegisterLayerUpdater).FullName));
            btnRegisterLayerUpdater.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "RegisterLayerUpdater32.png"), UriKind.Absolute));
            btnRegisterLayerUpdater.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "RegisterLayerUpdater16.png"), UriKind.Absolute));
            btnRegisterLayerUpdater.ToolTip = "Click once for auto updating of all layers tags";
            btnRegisterLayerUpdater.LongDescription = "Для автоматического обновления выносок надо нажать один раз при каждом открытии модели";

            PushButton btnLayerTagRefresh = (PushButton)panelTag.AddItem(new PushButtonData("Refresh Tag", "Refresh Tag", dllArCm, typeof(LayerTagRefresh).FullName));
            btnLayerTagRefresh.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnLayerTagRefresh.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnLayerTagRefresh.ToolTip = "Update all layers tags";

            string dllBerg = Path.Combine(folderPath, "StudyTask.dll");
            string tabBerg = "BIT";
            application.CreateRibbonTab(tabBerg);
            RibbonPanel panelBerg = application.CreateRibbonPanel(tabBerg, "BIT");

            PushButton btnCubeFamily = (PushButton)panelBerg.AddItem(new PushButtonData("CubeFamily", "CubeFamily", dllBerg, typeof(CubeFamilyCommand).FullName));
            btnCubeFamily.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnCubeFamily.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnCubeFamily.ToolTip = "";

            PushButton btnBlendFamily = (PushButton)panelBerg.AddItem(new PushButtonData("BlendFamily", "BlendFamily", dllBerg, typeof(BlendFamilyCommand).FullName));
            btnBlendFamily.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnBlendFamily.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnBlendFamily.ToolTip = "";

            PushButton btnSweepFamily = (PushButton)panelBerg.AddItem(new PushButtonData("SweepFamily", "SweepFamily", dllBerg, typeof(SweepFamilyCommand).FullName));
            btnSweepFamily.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnSweepFamily.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnSweepFamily.ToolTip = "";

            PushButton btnSweptBlendFamily = (PushButton)panelBerg.AddItem(new PushButtonData("SweptBlendFamily", "SweptBlendFamily", dllBerg, typeof(SweptBlendFamilyCommand).FullName));
            btnSweptBlendFamily.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnSweptBlendFamily.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnSweptBlendFamily.ToolTip = "";

            PushButton btnRevolutionFamily = (PushButton)panelBerg.AddItem(new PushButtonData("RevolutionFamily", "RevolutionFamily", dllBerg, typeof(RevolveFamilyCommand).FullName));
            btnRevolutionFamily.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnRevolutionFamily.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnRevolutionFamily.ToolTip = "";

            PushButton btnCopyFamily = (PushButton)panelBerg.AddItem(new PushButtonData("CopyFamily", "CopyFamily", dllBerg, typeof(CopyFamilyCommand).FullName));
            btnCopyFamily.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnCopyFamily.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnCopyFamily.ToolTip = "";

            PushButton btnFamilyExport = (PushButton)panelBerg.AddItem(new PushButtonData("Export", "Export", dllBerg, typeof(DocExport).FullName));
            btnFamilyExport.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnFamilyExport.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnFamilyExport.ToolTip = "";

            PushButton btnFamilyImport = (PushButton)panelBerg.AddItem(new PushButtonData("FamilyImport", "FamilyImport", dllBerg, typeof(FamilyImport).FullName));
            btnFamilyImport.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnFamilyImport.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnFamilyImport.ToolTip = "";

            PushButton btnProjectImport = (PushButton)panelBerg.AddItem(new PushButtonData("ProjectImport", "ProjectImport", dllBerg, typeof(ProjectImport).FullName));
            btnProjectImport.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnProjectImport.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnProjectImport.ToolTip = "";

            PushButton btnTest = (PushButton)panelBerg.AddItem(new PushButtonData("TestVoid", "TestVoid", dllBerg, typeof(TestVoid).FullName));
            btnTest.LargeImage = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh32.png"), UriKind.Absolute));
            btnTest.Image = new BitmapImage(new Uri(Path.Combine(folderImage, "LayerTagRefresh16.png"), UriKind.Absolute));
            btnTest.ToolTip = "";
        }
	}
}