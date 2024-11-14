using MathNet.Numerics.Integration;
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
            //DrawCircle();
            DrawWire();
        }

        private void DrawWire()
        {
            const int segments = 32; // Количество сегментов для круга
            const double R = 1; // Радиус круга
            const double b = 0.25; // Радиус волокна
            const double angle_step = Math.PI * 2 / segments;

            // Коэффициенты для линейной функции нахождения соотношения изменения верхней части волокна и нижней (при деформации)
            // верх = b1*x + b0
            // низ = верх + 1 (b1*x + [b0+1])
            const double b1 = -0.0189;
            const double b0 = 0.519;

            const double coeff_verx = b1 * b + b0;
            const double coeff_niz = coeff_verx + 1;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            double Alpha_angle = 80;
            double h = 0.1;
            double dlina_wire = 100;
            double dlina_prodolzhenie = 10;
            double dlina = dlina_wire + 2 * dlina_prodolzhenie;
            int dlina_count = (int)(dlina_wire/ h);
            double h_for_angle = Alpha_angle / dlina_count;
            double step = (Alpha_angle / dlina_count) * 0.01745;

            double angle_for_rotation = -90 * 0.01745;
            double angle_for_wire = 0;
            double tmp_angle = angle_for_rotation;
            double tmp_rot_x = 0;
            double tmp_rot_y = 0;

            double h_for_perehoda = Math.PI / (Alpha_angle * 0.01745);
            for (int i = 0; i <= (dlina / h); i++)
            {
                if ((i >= 0 && i <= dlina_prodolzhenie / h) || (i >= (dlina_wire + dlina_prodolzhenie) / h))
                {
                    if (i >= (dlina_prodolzhenie + dlina_wire) / h)
                    {
                        tmp_rot_x += Math.Sin(angle_for_rotation - 1.5708) * h;
                        tmp_rot_y -= Math.Cos(angle_for_rotation - 1.5708) * h;
                    }
                    else
                    {
                        tmp_rot_x = R + b - i * h * Math.Cos(Alpha_angle * 0.01745);
                        tmp_rot_y = -i * h;
                    }

                    positions.Add(new Point3D(tmp_rot_x, tmp_rot_y, 0));
                    for (int j = 0; j <= segments; j++)
                    {
                        double circle_angle = j * angle_step;

                        double circle_x = 0;
                        double circle_y = b * Math.Sin(circle_angle);
                        double circle_z = b * Math.Cos(circle_angle);

                        double after_rotation_x = Math.Cos(angle_for_rotation) * circle_x - Math.Sin(angle_for_rotation) * circle_y;
                        double after_rotation_y = Math.Sin(angle_for_rotation) * circle_x + Math.Cos(angle_for_rotation) * circle_y;
                        double after_rotation_z = circle_z;

                        positions.Add(new Point3D(after_rotation_x + tmp_rot_x, tmp_rot_y + after_rotation_y, after_rotation_z));
                    }
                }
                else
                {
                    double rotation_x = (R + b) * Math.Cos(angle_for_wire);
                    double rotation_y = (R + b) * Math.Sin(angle_for_wire);

                    positions.Add(new Point3D(rotation_x, rotation_y, 0));
                    for (int j = 0; j <= segments; j++)
                    {
                        double circle_angle = j * angle_step;

                        double r;
                        if (circle_angle < 3.1415) r = b * (1 - coeff_verx) * Math.Sin(circle_angle);
                        else r = b * coeff_verx * Math.Sin(circle_angle);

                        double r_for_perehod = b - Math.Sin(h_for_perehoda * angle_for_wire) * r;

                        double circle_x = 0;
                        double circle_y = r_for_perehod * Math.Sin(circle_angle);
                        double circle_z = r_for_perehod * Math.Cos(circle_angle);

                        double after_rotation_x = Math.Cos(angle_for_rotation) * circle_x - Math.Sin(angle_for_rotation) * circle_y;
                        double after_rotation_y = Math.Sin(angle_for_rotation) * circle_x + Math.Cos(angle_for_rotation) * circle_y;
                        double after_rotation_z = circle_z;


                        positions.Add(new Point3D(after_rotation_x + rotation_x, rotation_y + after_rotation_y, after_rotation_z));
                    }

                    angle_for_rotation += step;
                    angle_for_wire += step;

                    tmp_rot_x = rotation_x;
                    tmp_rot_y = rotation_y;
                }
            }

            //double h_for_perehoda = Math.PI / (Alpha_angle * 0.01745);
            //double angle_for_rotation = -90 * 0.01745;
            //double angle_for_wire = 0;
            
            //for (int i = 0; i <= dlina_count; i++)
            //{
            //    double rotation_x = (R + b) * Math.Cos(angle_for_wire);
            //    double rotation_y = (R + b) * Math.Sin(angle_for_wire);

            //    positions.Add(new Point3D(rotation_x, rotation_y, 0));
            //    for (int j = 0; j <= segments; j++)
            //    {
            //        double circle_angle = j * angle_step;

            //        double r;
            //        if (circle_angle < 3.1415) r = b * (1 - coeff_verx) * Math.Sin(circle_angle);
            //        else r = b * coeff_verx * Math.Sin(circle_angle);

            //        double r_for_perehod = b - Math.Sin(h_for_perehoda * angle_for_wire) * r;

            //        double circle_x = 0;
            //        double circle_y = r_for_perehod * Math.Sin(circle_angle);
            //        double circle_z = r_for_perehod * Math.Cos(circle_angle);

            //        double after_rotation_x = Math.Cos(angle_for_rotation) * circle_x - Math.Sin(angle_for_rotation) * circle_y;
            //        double after_rotation_y = Math.Sin(angle_for_rotation) * circle_x + Math.Cos(angle_for_rotation) * circle_y;
            //        double after_rotation_z = circle_z;


            //        positions.Add(new Point3D(after_rotation_x + rotation_x, rotation_y + after_rotation_y, after_rotation_z));
            //    }

            //    angle_for_rotation += step;
            //    angle_for_wire += step;
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

            mesh.Positions = positions;
            mesh.TriangleIndices = triangleIndices;

            // Создаем материал и модель
            Material material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue));
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            RotateTransform3D transform = new RotateTransform3D();
            AxisAngleRotation3D axis = new AxisAngleRotation3D();
            this.RegisterName("rotate", axis);
            transform.Rotation = axis;
            visual.Transform = transform;
            visual.Content = model;
            viewport.Children.Add(visual);
        }


        private void DrawCircle()
        {
            const int segments = 32; // Количество сегментов для круга
            const double R = 1; // Радиус круга
            const double b = 0.5; // Радиус волокна
            const double angle_step = Math.PI * 2 / segments;

            // Коэффициенты для линейной функции нахождения соотношения изменения верхней части волокна и нижней (при деформации)
            // верх = b1*x + b0
            // низ = верх + 1 (b1*x + [b0+1])
            const double b1 = -0.0189;
            const double b0 = 0.519;

            const double coeff_verx = b1 * b + b0;
            const double coeff_niz = coeff_verx + 1;

            const int quarter_of_circle = segments / 4;

            const double h_quarter_1 = (coeff_verx * b - b) / quarter_of_circle;
            const double h_quarter_2 = (b - coeff_verx * b) / quarter_of_circle;
            const double h_quarter_3 = (coeff_niz * b - b) / quarter_of_circle;
            const double h_quarter_4 = (b - coeff_niz * b) / quarter_of_circle;

            //double tmp_b = b;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            double Alpha_angle = 45;
            double Alpha_angle_rad = 0.0175 * Alpha_angle;

            //double b1_ = -(coeff_verx * b) / 900 - b / 450 + (coeff_niz * b) / 300;
            //double b0_ = 0.8 * b + 0.4 * coeff_verx * b - 0.2 * coeff_niz * b;

            Console.WriteLine(coeff_verx);
            Console.WriteLine(coeff_niz);
            //double h_for_angle = 2 * Math.PI / Alpha_angle_rad;
            positions.Add(new Point3D(0, 0, 0));
            // Создаем вершины круга
            for (int i = 0; i <= segments; i++)
            {
                double angle = i * angle_step;
                double r;
                //double tmp_b = b1_ * angle + b0_;
                if (angle < 3.1415) r = (b * (1 - coeff_verx)) * Math.Sin(angle);
                else r = (b * coeff_verx) * Math.Sin(angle);
                Console.WriteLine($"{angle}, {Math.Sin(angle)}");
                //double r_for_perehod = b - Math.Sin(h_for_perehoda * angle_for_wire) * r;
                double x = (b - r) * Math.Cos(angle);
                double y = (b - r) * Math.Sin(angle);
                double z = 0;
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
