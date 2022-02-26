using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIUI1
{
    public class MainViewViewModel
    {
        private readonly ExternalCommandData _commandData;

        public DelegateCommand SelectCommandPipe { get; }
        public DelegateCommand SelectCommandWall { get; }
        public DelegateCommand SelectCommandDoor { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SelectCommandPipe = new DelegateCommand(OnSelectCommandPipe);
            SelectCommandWall = new DelegateCommand(OnSelectCommandWall);
            SelectCommandDoor = new DelegateCommand(OnSelectCommandDoor);
        }

        public event EventHandler CloseRequest;

        private void RaiseCloseRequest() 
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectCommandPipe()
        {
            RaiseCloseRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipes = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfClass(typeof(Pipe))
                .Cast<Pipe>()
                .ToList();

            TaskDialog.Show("Сообщение", $"Количество труб на виде: {pipes.Count}");

        }

        private void OnSelectCommandWall()
        {
            RaiseCloseRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            var walls = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfClass(typeof(Wall))
                .Cast<Wall>()
                .ToList();

            var volume = new List<double>();
            double sum = 0;
            string Vol = string.Empty;

            foreach (var wall in walls)
            {
                Parameter wallVolume = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                double V = UnitUtils.ConvertFromInternalUnits(wallVolume.AsDouble(), UnitTypeId.CubicMeters);
                volume.Add(V);
            }
            foreach (var i in volume)
            {
                sum += i;
            }
            string SumVolume = sum.ToString();
            Vol += $"Объем: {SumVolume}м3{Environment.NewLine}Общее количество элементов стен: {walls.Count}";

            TaskDialog.Show("Сообщение", Vol);

        }

        private void OnSelectCommandDoor()
        {
            RaiseCloseRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var doors = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            TaskDialog.Show("Сообщение", $"Количество дверей на виде: {doors.Count}");

        }
    }
}
