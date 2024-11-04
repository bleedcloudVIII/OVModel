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

        private List<double> calcCoordsRotationY(double R, double rotationAngle, double x, double y, double z)
        {
            return new List<double>()
            {
                R * (Math.Cos(rotationAngle) * x + Math.Sin(rotationAngle) * z),
                R * y,
                -1 * Math.Sin(rotationAngle) * x + Math.Cos(rotationAngle) * z
            };
        }

        private List<double> calcCoordsRotationZ(double R, double rotationAngle, double x, double y, double z)
        {
            return new List<double>()
            {
                R * (Math.Cos(rotationAngle) * x - Math.Sin(rotationAngle) * y),
                R * (Math.Sin(rotationAngle) * x + Math.Cos(rotationAngle * y)),
                z,
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
            const double R = 1; // Радиус круга
            const double b = 0.5;
            const double angle_step = Math.PI * 2 / segments;
            
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            double Alpha_angle = 180;
            double h = 0.1;
            double dlina = 100;
            int dlina_count = (int)(dlina/ h);
            double h_for_angle = Alpha_angle / dlina_count;
            double step = (Alpha_angle * Math.PI) / (180 * dlina_count);
            // Создаем вершины круга


            //for (int i = 0; i <= dlina_count; i++)
            //{
            //    double rotation_angle = i * step;
            //    double rotation_x = (R + b) * Math.Cos(rotation_angle);
            //    double rotation_y = (R + b) * Math.Sin(rotation_angle);

            //    positions.Add(new Point3D(rotation_x, rotation_y, 0));
            //    for (int j = 0; j < segments; j++)
            //    {
            //        double circle_angle = j * angle_step;
            //        double circle_z = Math.Cos(circle_angle);
            //        double circle_y = Math.Sin(circle_angle);
            //        double circle_x = i * h;

            //        double f_x = circle_x * Math.Cos(rotation_angle) + Math.Sin(rotation_angle) * circle_x;
            //        double f_y = rotation_y + b * circle_y;
            //        double f_z = rotation_x + b * (- Math.Sin(rotation_angle) * circle_z + Math.Cos(rotation_angle) * circle_x);
            //        positions.Add(new Point3D(f_x, f_y, f_z));


            //        //List<double> c = calcCoordsRotationZ(b, rotation_angle, circle_x, circle_y, circle_z);
            //        //positions.Add(new Point3D(rotation_x + c[0], rotation_y + c[1], c[2]));
            //        //positions.Add(new Point3D(rotation_x + b * circle_x, b * circle_y, rotation_z + circle_z));
            //    }
            //}
            double angle_test = Alpha_angle;
            double angle_for_rotation = angle_test;
            double angle_for_wire =  (90 - Alpha_angle / 2) * 0.01745;
            for (int i = 0; i <= dlina_count; i++)
            {
                //double rotation_angle = i * step;
                double rotation_x = (R + b) * Math.Cos(angle_for_wire);
                double rotation_y = (R + b) * Math.Sin(angle_for_wire);

                positions.Add(new Point3D(rotation_x, rotation_y, 0));
                for (int j = 0; j < segments; j++)
                {
                    double circle_angle = j * angle_step;
                    double circle_x = rotation_x;
                    double circle_y = rotation_y + b * Math.Sin(circle_angle);
                    double circle_z = b * Math.Cos(circle_angle);

                    //double after_rotation_x = Math.Cos(angle_for_rotation) * circle_x - Math.Sin(angle_for_rotation) * circle_y;
                    //double after_rotation_y = Math.Sin(angle_for_rotation) * circle_x + Math.Cos(angle_for_rotation) * circle_y;
                    //double after_rotation_z = circle_z;

                    positions.Add(new Point3D(circle_x, circle_y, circle_z));

                    //List<double> c = calcCoordsRotationY(b, rotation_angle, circle_z, circle_y, circle_x);
                    //positions.Add(new Point3D(after_rotation_x, after_rotation_y, after_rotation_z));
                    //positions.Add(new Point3D(rotation_x + b * circle_x, b * circle_y, rotation_z + circle_z));
                }

                angle_for_rotation -= step;
                angle_for_wire += step;
            }

            //for (double j = 0; j <= dlina_count; j += 1)
            //{
            //    double rotationAngle = j * step;
            //    double centr_x = (R + b) * Math.Cos(rotationAngle);
            //    double centr_z = (R + b) * Math.Sin(rotationAngle);
            //    positions.Add(new Point3D(centr_x, 0, centr_z));
            //    for (int i = 0; i < segments; i++)
            //    {
            //        double angle = i * angle_step;
            //        double y = Math.Cos(angle);
            //        double z = Math.Sin(angle);
            //        double x = j * h;
            //        List<double> coords = calcCoordsRotationY(b, rotationAngle, y, z, x);
            //        positions.Add(new Point3D(coords[0], coords[1], coords[2]));
            //        //positions.Add(new Point3D(centr_x + b * x, b * y, centr_z + z));

            //    }
            //}

            int segmentsOnEveryCircle = segments + 1;

            for (int z_layer = 0; z_layer < (dlina / h) - 1; z_layer++)
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
