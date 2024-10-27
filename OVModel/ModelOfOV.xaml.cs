using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace OVModel
{
    /// <summary>
    /// Логика взаимодействия для ModelOfOV.xaml
    /// </summary>
    public partial class ModelOfOV : Window
    {
        public ModelOfOV()
        {
            InitializeComponent();
            DrawWire();
        }


        private void DrawWire()
        {
            const int segments = 100; // Количество сегментов для окружности

            const double radius = 1.0; // Радиус окружности


            MeshGeometry3D mesh = new MeshGeometry3D();

            Point3DCollection positions = new Point3DCollection();

            Int32Collection triangleIndices = new Int32Collection();


            // Создаем вершины окружности

            for (int i = 0; i < segments; i++)

            {

                double angle = i * (Math.PI * 2 / segments);

                double x = radius * Math.Cos(angle);

                double z = radius * Math.Sin(angle);

                positions.Add(new Point3D(x, 0, z)); // y = 0 для плоской окружности

            }


            // Добавляем центральную точку (0, 0, 0)

            positions.Add(new Point3D(0, 0, 0));


            // Создаем треугольники

            for (int i = 0; i < segments; i++)

            {

                triangleIndices.Add(segments); // Индекс центра

                triangleIndices.Add(i);         // Индекс текущей точки окружности

                triangleIndices.Add((i + 1) % segments); // Индекс следующей точки окружности

            }


            mesh.Positions = positions;
            mesh.TriangleIndices = triangleIndices;


            // Создаем материал и модель
            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));
            GeometryModel3D model = new GeometryModel3D(mesh, material);


            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;
            mainViewPort.Children.Add(visual);
        }


        private void DrawCircle()
        {
            const int segments = 50; // Количество сегментов для круга
            const double radius = 1.0; // Радиус круга
            const double angleStep = Math.PI * 2 / segments;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();


            // Создаем вершины круга
            for (int i = 0; i <= segments; i++)
            {
                double angle = i * angleStep;
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);
                positions.Add(new Point3D(x, y, 0));
            }


            // Создаем треугольники
            for (int i = 0; i < segments; i++)
            {
                triangleIndices.Add(0); // Центр (начальная точка)
                triangleIndices.Add(i + 1);
                triangleIndices.Add(i + 2);
            }

            mesh.Positions = positions;
            mesh.TriangleIndices = triangleIndices;

            // Создаем материал и модель
            Material material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue));
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;
            mainViewPort.Children.Add(visual);
        }

        private void AddCircleModel()
        {
            var mod = GetCircleModel(1, new Vector3D(0, 1, 1), new Point3D(0, 0, 0), 40);
            mod.Material = new DiffuseMaterial(Brushes.Silver);
            var vis = new ModelVisual3D() { Content = mod };
            mainViewPort.Children.Add(vis);
        }

        private GeometryModel3D GetCircleModel(double radius, Vector3D normal, Point3D center, int resolution)
        {
            var mod = new GeometryModel3D();
            var geo = new MeshGeometry3D();

            // Generate the circle in the XZ-plane
            // Add the center first
            geo.Positions.Add(new Point3D(0, 0, 0));

            // Iterate from angle 0 to 2*PI
            double t = 2 * Math.PI / resolution;
            for (int i = 0; i < resolution; i++)
            {
                geo.Positions.Add(new Point3D(radius * Math.Cos(t * i), 0, -radius * Math.Sin(t * i)));
            }

            // Add points to MeshGeoemtry3D
            for (int i = 0; i < resolution; i++)
            {
                var a = 0;
                var b = i + 1;
                var c = (i < (resolution - 1)) ? i + 2 : 1;

                geo.TriangleIndices.Add(a);
                geo.TriangleIndices.Add(b);
                geo.TriangleIndices.Add(c);
            }

            mod.Geometry = geo;

            // Create transforms
            var trn = new Transform3DGroup();
            // Up Vector (normal for XZ-plane)
            var up = new Vector3D(0, 1, 0);
            // Set normal length to 1
            normal.Normalize();
            var axis = Vector3D.CrossProduct(up, normal); // Cross product is rotation axis
            var angle = Vector3D.AngleBetween(up, normal); // Angle to rotate
            trn.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, angle)));
            trn.Children.Add(new TranslateTransform3D(new Vector3D(center.X, center.Y, center.Z)));

            mod.Transform = trn;

            return mod;
        }
    }
}
