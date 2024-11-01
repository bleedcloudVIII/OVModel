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
using System.Windows.Media.Animation;
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
            //DrawWire();
        }

        private List<double> calcCoords(double R, double rotationAngle, double x, double y, double z)
        {
            return new List<double>()
            {
                R * (Math.Cos(rotationAngle) * x + Math.Sin(rotationAngle) * z),
                R * y,
                -1 * Math.Sin(rotationAngle) * x + Math.Cos(rotationAngle) * z
            };
        }

        //private List<double> testc(double R, double z, double theta, )
        //{
        //    double theta = (2 * Math.PI / pointsPerCirle) * j; // равномерные углы
        //    double x = R * Math.Cos(theta); // координата X
        //    double y = R * Math.Sin(theta); // координата Y
        //}
        private void DrawWire()
        {
            const int segments = 30; // Количество сегментов для круга
            const double R = 0.5; // Радиус круга
            const double angleStep = Math.PI * 2 / segments;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            double Alpha_angle = 45;
            double h_for_z = 0.1;
            double z_max = 10;
            int z_count = (int)(z_max / h_for_z);
            double h_for_angle = Alpha_angle / z_count;

            // Создаем вершины круга


            for (double z = 0; z < z_count; z += 1)
            {
                // Точка центра окружности
                double rotationAngle = 0.0175 * z * h_for_angle;
                //double rotationAngle = 45;
                positions.Add(new Point3D(0, 0, z * h_for_z));
                for (int i = 0; i < segments; i++)
                {
                    double angle = i * angleStep;
                    double x = Math.Cos(angle);
                    double y = Math.Sin(angle);
                    List<double> coords = calcCoords(R, rotationAngle, x, y, z * h_for_z);
                    //Console.WriteLine($"{x}, {y}, {z}");
                    //positions.Add(new Point3D(x, y, z));
                    //Console.WriteLine($"{z} {h_for_z} {h_for_angle} {rotationAngle} {coords[0]} {coords[1]} {coords[2]}");
                    positions.Add(new Point3D(coords[0], coords[1], coords[2]));
                }
            }

            int segmentsOnEveryCircle = segments + 1;

            for (int z_layer = 0; z_layer < (z_max / h_for_z) - 1; z_layer++)
            {
                for (int i = 0; i < segments; i++)
                {
                    if (i != segments - 1)
                    {

                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 2);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + i + 2);

                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 2);
                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + i + 2);

                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + i + 2);

                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + i + 2);

                    }
                    else
                    {
                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + 1);

                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + 1);

                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + 1);

                        triangleIndices.Add(segmentsOnEveryCircle * z_layer + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + i + 1);
                        triangleIndices.Add(segmentsOnEveryCircle * (z_layer + 1) + 1);
                    }
                }
            }


            //triangleIndices.Add(1);
            //triangleIndices.Add(12);
            //triangleIndices.Add(13);

            //triangleIndices.Add(2);
            //triangleIndices.Add(1);
            //triangleIndices.Add(13);
            mesh.Positions = positions;
            mesh.TriangleIndices = triangleIndices;

            // Создаем материал и модель
            Material material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue));
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            RotateTransform3D transform = new RotateTransform3D();
            AxisAngleRotation3D axis = new AxisAngleRotation3D();
            //axis.SetValue(Name, "rotate");
            this.RegisterName("rotate", axis);
            transform.Rotation = axis;
            visual.Transform = transform;
            visual.Content = model;
            viewport.Children.Add(visual);
        }


        private void DrawCircle()
        {
            const int segments = 50; // Количество сегментов для круга
            const double radius = 1.0; // Радиус круга
            const double angleStep = Math.PI * 2 / segments;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            double Alpha_angle = 45;
            double Alpha_angle_rad = 0.0175 * Alpha_angle;

            // Создаем вершины круга
            for (int i = 0; i <= segments; i++)
            {
                double angle = i * angleStep;
                double x = radius * Math.Cos(angle) * Math.Cos(Alpha_angle_rad) + Math.Sin(Alpha_angle_rad) * 0; ;
                double y = radius * Math.Sin(angle);
                double z = -Math.Sin(Alpha_angle_rad) * Math.Cos(angle) + Math.Cos(Alpha_angle_rad) * 0;
                positions.Add(new Point3D(x, y, z));
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
            viewport.Children.Add(visual);
        }
    }
}
