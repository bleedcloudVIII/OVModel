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
using OVModel.Lib.UserInput;
using OVModel.Lib.CommonClasses;
using System.ComponentModel.Composition;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Security.Cryptography;
using MathNet.Numerics.Distributions;
using System.Security.Policy;

namespace OVModel
{
    /// <summary>
    /// Логика взаимодействия для ModelOfOV.xaml
    /// </summary>
    public partial class ModelOfOV : Window
    {
        private UserInput userInput;
        private int segments = 32;

        // Коэффициенты для линейной функции нахождения соотношения изменения верхней части волокна и нижней (при деформации)
        // верх = b1*x + b0
        // низ = верх + 1 (b1*x + [b0+1])
        private const double b1 = -0.0189;
        private const double b0 = 0.519;

        // Вращение камеры
        private Point lastMousePosition;
        private bool isRotating;
        private double angleX = 0.0;
        private double angleY = 0.0;
        private Vector3D cameraPosition = new Vector3D(0, 0, 5);
        private double KeyMoveSpeed = 0.3;
        private double mouseMoveSpeed = 0.3;

        // Приближение/отдаление камеры
        private double zoomChange = 0;
        private const double ZoomFactor = 0.5;

        // Оси
        double coef_for_axis = 4;
        double coef_for_axis2D = 1.5;
        double line_2D = 0.005;
        public ModelOfOV(UserInput uI)
        {
            userInput = uI;
            InitializeComponent();

            DrawCilindr();
            DrawWire();
            DrawSrez();
            DrawUserSrez((int)uI.Alpha / 2);

            CreateAxis();
            CreateAxis2D();

            Input_Betta.Text = $"{(int)(uI.Alpha / 2)}";
            DrawCircle((int)(uI.Alpha / 2));
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isRotating = true;
                lastMousePosition = e.GetPosition(this);
                Mouse.Capture(this);
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                cameraPosition += camera_3d.LookDirection * KeyMoveSpeed;
            }
            else if (e.Key == Key.S)
            {
                cameraPosition -= camera_3d.LookDirection * KeyMoveSpeed;
            }
            else if (e.Key == Key.A)
            {
                Vector3D right = Vector3D.CrossProduct(camera_3d.LookDirection, new Vector3D(0, 1, 0));
                cameraPosition -= right * KeyMoveSpeed;
            }
            else if (e.Key == Key.D)
            {
                Vector3D left = Vector3D.CrossProduct(camera_3d.LookDirection, new Vector3D(0, 1, 0));
                cameraPosition += left * KeyMoveSpeed;
            }
            else if (e.Key == Key.LeftShift)
            {
                cameraPosition.Y += KeyMoveSpeed;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                cameraPosition.Y -= KeyMoveSpeed;
            }

            camera_3d.Position = new Point3D(cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRotating)
            {
                Point currentMousePosition = e.GetPosition(this);
                Vector delta = currentMousePosition - lastMousePosition;

                double deltaX = delta.X * mouseMoveSpeed; // Угол по Y
                double deltaY = delta.Y * mouseMoveSpeed; // Угол по X

                RotateCamera(deltaX, deltaY);

                lastMousePosition = currentMousePosition;
            }
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                isRotating = false;
                Mouse.Capture(null);
            }
        }
        private void RotateCamera(double deltaX, double deltaY)
        {
            angleX += deltaY; // Угол по оси X
            angleY += deltaX; // Угол по оси Y

            Vector3D lookDirection = new Vector3D(
                Math.Sin(angleY * Math.PI / 180) * Math.Cos(angleX * Math.PI / 180),
                -Math.Sin(angleX * Math.PI / 180),
                -Math.Cos(angleY * Math.PI / 180) * Math.Cos(angleX * Math.PI / 180)
            );


            camera_3d.LookDirection = lookDirection;
        }
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Vector3D direction = camera_3d.LookDirection;
            direction.Normalize();

            double zoomAmount = e.Delta > 0 ? 0.5 : -0.5;

            camera_3d.Position += direction * zoomAmount;

            cameraPosition = new Vector3D(camera_3d.Position.X, camera_3d.Position.Y, camera_3d.Position.Z);
        }

        private void DrawWire()
        {
            double R = userInput.R; // Радиус круга
            double b = userInput.b; // Радиус волокна
            double angle_step = Math.PI * 2 / segments;

            double coeff_verx = b1 * b + b0;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            Point3DCollection positions_for_line = new Point3DCollection();
            Int32Collection triangleIndices_for_line = new Int32Collection();

            double Alpha_angle = userInput.Alpha;
            double h = 0.1;
            double dlina_wire = 2;
            double dlina_prodolzhenie = 10;
            double dlina = dlina_wire + 2 * dlina_prodolzhenie;
            int dlina_count = (int)(dlina_wire/ h);
            
            double step = (Alpha_angle / dlina_count) * 0.01745;

            double angle_for_rotation = -90 * 0.01745;
            double angle_for_wire = 0;


            double tmp_angle = angle_for_rotation;
            double tmp_rot_x = R + b;
            double tmp_rot_y = 0;

            double x_0 = R + b;
            double y_0 = 0;
            double x_1 = (R + 2 * b) * Math.Cos(Alpha_angle * 0.01745 / 2);
            double y_1 = (R + 2 * b) * Math.Sin(Alpha_angle * 0.01745 / 2);

            double h_for_x_1_prodolzhenie = x_1 - x_0;
            double h_for_y_1_prodolzhenie = y_1 - y_0;

            double centr_line_x;
            double centr_line_y;

            double tmp_centr_line_x = 0;
            double tmp_centr_line_y = 0;

            int curr = 0;

            bool isNeedSoedinyat = false;

            double h_for_perehoda = Math.PI / (Alpha_angle * 0.01745);
            for (int i = 0; i <= (dlina / h); i++)
            {
                if ((i >= 0 && i <= dlina_prodolzhenie / h) || (i >= (dlina_wire + dlina_prodolzhenie) / h))
                {
                    int lyam = i < (dlina_prodolzhenie + dlina_wire) / h ? lyam = -1 : lyam = 1;

                    tmp_rot_x += Math.Sin(lyam * angle_for_rotation - 1.5708) * h;
                    tmp_rot_y -= Math.Cos(lyam * angle_for_rotation - 1.5708) * h;

                    centr_line_x = tmp_rot_x;
                    centr_line_y = tmp_rot_y;

                    positions.Add(new Point3D(tmp_rot_x, tmp_rot_y, 0));
                    for (int j = 0; j < segments; j++)
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

                    centr_line_x = rotation_x;
                    centr_line_y = rotation_y;

                    positions.Add(new Point3D(rotation_x, rotation_y, 0));
                    for (int j = 0; j < segments; j++)
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

                if (isNeedSoedinyat)
                {
                    positions_for_line.Add(new Point3D(centr_line_x, centr_line_y, 0.05));
                    positions_for_line.Add(new Point3D(tmp_centr_line_x, tmp_centr_line_y, 0.05));
                    positions_for_line.Add(new Point3D(tmp_centr_line_x, tmp_centr_line_y, -0.05));
                    positions_for_line.Add(new Point3D(centr_line_x, centr_line_y, -0.05));

                    triangleIndices_for_line.Add(curr * 4);
                    triangleIndices_for_line.Add(curr * 4 + 1);
                    triangleIndices_for_line.Add(curr * 4 + 2);

                    triangleIndices_for_line.Add(curr * 4);
                    triangleIndices_for_line.Add(curr * 4 + 2);
                    triangleIndices_for_line.Add(curr * 4 + 1);

                    triangleIndices_for_line.Add(curr * 4);
                    triangleIndices_for_line.Add(curr * 4 + 2);
                    triangleIndices_for_line.Add(curr * 4 + 3);

                    triangleIndices_for_line.Add(curr * 4);
                    triangleIndices_for_line.Add(curr * 4 + 3);
                    triangleIndices_for_line.Add(curr * 4 + 2);

                    curr++;

                    isNeedSoedinyat = false;

                }

                tmp_centr_line_x = centr_line_x;
                tmp_centr_line_y = centr_line_y;

                if (i % 2 == 1) isNeedSoedinyat = true;
            }

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

            var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray);
            brush.Opacity = 0.7;
            Material material = new DiffuseMaterial(brush);

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            

            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            RotateTransform3D transform = new RotateTransform3D();

            AxisAngleRotation3D axis = new AxisAngleRotation3D();

            transform.Rotation = axis;
            
            visual.Content = model;
            visual.Transform = transform;


            var line = new MeshGeometry3D
            {
                Positions = positions_for_line,
                TriangleIndices = triangleIndices_for_line,
            };

            var material_line = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black));
            var lineModel = new GeometryModel3D(line, material_line);
            var modelVisual = new ModelVisual3D { Content = lineModel };

            viewport_3d.Children.Add(modelVisual);

            viewport_3d.Children.Add(visual);
        }

        private void CreateAxis2D()
        {
            // X пунктир
            var viewport_for_text = new Viewport2DVisual3D();

            var p = new Point3DCollection
            {
                new Point3D(-userInput.b, line_2D, 0),
                new Point3D(userInput.b, line_2D, 0),
                new Point3D(userInput.b, -line_2D, 0),
                new Point3D(-userInput.b, -line_2D, 0),
            };

            var triangles = new Int32Collection
            {
                0, 1, 2,
                0, 2, 1,
                0, 2, 3,
                0, 3, 2,
            };

            var texture = new PointCollection
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0),
            };
            var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            var meshGeometry = new MeshGeometry3D() { Positions = p, TriangleIndices = triangles, TextureCoordinates = texture };
            var mat = new DiffuseMaterial();
            mat.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);

            var line = new Line() { StrokeThickness = 0.01, Stroke = brush, X1 = -userInput.b, Y1 = 0, X2 = 2 * userInput.b, Y2 = 0, StrokeDashArray = new DoubleCollection {6, 1} };

            viewport_for_text.Visual = line;
            viewport_for_text.Material = mat;
            viewport_for_text.Geometry = meshGeometry;

            viewport_2d.Children.Add(viewport_for_text);

            // X продолжение минус
            viewport_for_text = new Viewport2DVisual3D();

            p = new Point3DCollection
            {
                new Point3D(-2 * userInput.b, line_2D, 0),
                new Point3D(-userInput.b, line_2D, 0),
                new Point3D(-userInput.b, -line_2D, 0),
                new Point3D(-2 * userInput.b, -line_2D, 0),
            };

            triangles = new Int32Collection
            {
                0, 1, 2,
                0, 2, 1,
                0, 2, 3,
                0, 3, 2,
            };

            texture = new PointCollection
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0),
            };
            brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            meshGeometry = new MeshGeometry3D() { Positions = p, TriangleIndices = triangles, TextureCoordinates = texture };
            mat = new DiffuseMaterial();
            mat.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);

            line = new Line() { StrokeThickness = 0.01, Stroke = brush, X1 = -2 * userInput.b, Y1 = 0, X2 = - userInput.b, Y2 = 0 };

            viewport_for_text.Visual = line;
            viewport_for_text.Material = mat;
            viewport_for_text.Geometry = meshGeometry;

            viewport_2d.Children.Add(viewport_for_text);

            // X продолжение плюс
            viewport_for_text = new Viewport2DVisual3D();

            p = new Point3DCollection
            {
                new Point3D(userInput.b, line_2D, 0),
                new Point3D(2 * userInput.b, line_2D, 0),
                new Point3D(2 * userInput.b, -line_2D, 0),
                new Point3D(userInput.b, -line_2D, 0),
            };

            triangles = new Int32Collection
            {
                0, 1, 2,
                0, 2, 1,
                0, 2, 3,
                0, 3, 2,
            };

            texture = new PointCollection
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0),
            };
            brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            meshGeometry = new MeshGeometry3D() { Positions = p, TriangleIndices = triangles, TextureCoordinates = texture };
            mat = new DiffuseMaterial();
            mat.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);

            line = new Line() { StrokeThickness = 0.01, Stroke = brush, X1 = userInput.b, Y1 = 0, X2 = 2 * userInput.b, Y2 = 0 };

            viewport_for_text.Visual = line;
            viewport_for_text.Material = mat;
            viewport_for_text.Geometry = meshGeometry;

            viewport_2d.Children.Add(viewport_for_text);

            // Y пунктир
            viewport_for_text = new Viewport2DVisual3D();

            p = new Point3DCollection
            {
                new Point3D(-line_2D, userInput.b, 0),
                new Point3D(line_2D, userInput.b, 0),
                new Point3D(line_2D, -userInput.b, 0),
                new Point3D(-line_2D, -userInput.b, 0),
            };

            triangles = new Int32Collection
            {
                0, 1, 2,
                0, 2, 1,
                0, 2, 3,
                0, 3, 2,
            };

            texture = new PointCollection
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0),
            };
            brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            meshGeometry = new MeshGeometry3D() { Positions = p, TriangleIndices = triangles, TextureCoordinates = texture };
            mat = new DiffuseMaterial();
            mat.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);

            line = new Line() { StrokeThickness = 0.01, Stroke = brush, X1 = 0, Y1 = -userInput.b, X2 = 0, Y2 = 2 * userInput.b, StrokeDashArray = new DoubleCollection { 6, 1 } };

            viewport_for_text.Visual = line;
            viewport_for_text.Material = mat;
            viewport_for_text.Geometry = meshGeometry;

            viewport_2d.Children.Add(viewport_for_text);

            // Y продолжение минус
            viewport_for_text = new Viewport2DVisual3D();

            p = new Point3DCollection
            {
                new Point3D(-line_2D, -userInput.b, 0),
                new Point3D(line_2D, -userInput.b, 0),
                new Point3D(line_2D, -2 * userInput.b, 0),
                new Point3D(-line_2D, -2 * userInput.b, 0),
            };

            triangles = new Int32Collection
            {
                0, 1, 2,
                0, 2, 1,
                0, 2, 3,
                0, 3, 2,
            };

            texture = new PointCollection
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0),
            };
            brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            meshGeometry = new MeshGeometry3D() { Positions = p, TriangleIndices = triangles, TextureCoordinates = texture };
            mat = new DiffuseMaterial();
            mat.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);

            line = new Line() { StrokeThickness = 0.01, Stroke = brush, X1 = 0, Y1 = -userInput.b, X2 = 0, Y2 = -2 * userInput.b };

            viewport_for_text.Visual = line;
            viewport_for_text.Material = mat;
            viewport_for_text.Geometry = meshGeometry;

            viewport_2d.Children.Add(viewport_for_text);

            // Y продолжение плюс
            viewport_for_text = new Viewport2DVisual3D();

            p = new Point3DCollection
            {
                new Point3D(-line_2D, userInput.b, 0),
                new Point3D(line_2D, userInput.b, 0),
                new Point3D(line_2D, 2 * userInput.b, 0),
                new Point3D(-line_2D, 2 * userInput.b, 0),
            };

            triangles = new Int32Collection
            {
                0, 1, 2,
                0, 2, 1,
                0, 2, 3,
                0, 3, 2,
            };

            texture = new PointCollection
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0),
            };
            brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            meshGeometry = new MeshGeometry3D() { Positions = p, TriangleIndices = triangles, TextureCoordinates = texture };
            mat = new DiffuseMaterial();
            mat.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);

            line = new Line() { StrokeThickness = 0.01, Stroke = brush, X1 = 0, Y1 = userInput.b, X2 = 0, Y2 = 2 * userInput.b };

            viewport_for_text.Visual = line;
            viewport_for_text.Material = mat;
            viewport_for_text.Geometry = meshGeometry;

            viewport_2d.Children.Add(viewport_for_text);
        }
        private void DrawCilindr()
        {
            double R = userInput.R; // Радиус круга
            double b = userInput.b; // Радиус волокна
            double angle_step = Math.PI * 2 / segments;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            double Alpha_angle = userInput.Alpha;
            double h = 0.1;
            double dlina = 2 * b + 1;
            int dlina_count = (int)(dlina / h);

            double z_start = - dlina / 2;

            for (int i = 0; i <= (dlina / h); i++)
            {
                double centr_x = 0;
                double centr_y = 0;
                double centr_z = z_start + i * h;

                positions.Add(new Point3D(centr_x, centr_y, centr_z));
                for (int j = 0; j < segments; j++)
                {
                    double circle_angle = j * angle_step;

                    double circle_x = R * Math.Cos(circle_angle);
                    double circle_y = R * Math.Sin(circle_angle);

                    positions.Add(new Point3D(circle_x + centr_x, circle_y + centr_y, centr_z));
                }
                
            }

            int segmentsOnEveryCircle = segments + 1;
            for (int z_layer = 0; z_layer < dlina_count - 1; z_layer++)
            {
                if (z_layer == 0)
                {
                    for (int i = 0; i < segments; i++)
                    {
                        if (i == segments - 1)
                        {
                            triangleIndices.Add(0);
                            triangleIndices.Add(i);
                            triangleIndices.Add(1);

                            triangleIndices.Add(0);
                            triangleIndices.Add(1);
                            triangleIndices.Add(i);
                        }
                        else
                        {
                            triangleIndices.Add(0);
                            triangleIndices.Add(i + 1);
                            triangleIndices.Add(i + 2);

                            triangleIndices.Add(0);
                            triangleIndices.Add(i + 2);
                            triangleIndices.Add(i + 1);
                        }
                    }
                }
                else if (z_layer == dlina_count - 2)
                {
                    for (int i = 0; i < segments; i++)
                    {
                        if (i == segments - 1)
                        {
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle);
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle + i);
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle + 1);

                            triangleIndices.Add(z_layer * segmentsOnEveryCircle);
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle + 1);
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle + i);
                        }
                        else
                        {
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle);
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle + i + 1);
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle + i + 2);

                            triangleIndices.Add(z_layer * segmentsOnEveryCircle);
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle + i + 2);
                            triangleIndices.Add(z_layer * segmentsOnEveryCircle + i + 1);
                        }
                    }
                }


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
            
            Material material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray));
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            RotateTransform3D transform = new RotateTransform3D();

            AxisAngleRotation3D axis = new AxisAngleRotation3D();

            //this.RegisterName("rotate", axis);

            transform.Rotation = axis;

            visual.Content = model;
            visual.Transform = transform;
            viewport_3d.Children.Add(visual);
        }
        private void DrawCircle(int angle_for_2D)
        {
            double R = userInput.R; // Радиус круга
            double b = userInput.b; // Радиус волокна
            double angle_step = Math.PI * 2 / segments;

            double coeff_verx = b1 * b + b0;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            double Alpha_angle = userInput.Alpha;

            //positions.Add(new Point3D(0, 0, 0));
            double h_for_perehoda = Math.PI / (Alpha_angle * 0.01745);
            for (int i = 0; i <= segments; i++)
            {
                double angle = i * angle_step;
                double r;

                if (angle < 3.1415) r = (b * (1 - coeff_verx)) * Math.Sin(angle);
                else r = (b * coeff_verx) * Math.Sin(angle);

                double r_for_perehod = b - Math.Sin(angle_for_2D * 0.01745 * h_for_perehoda) * r;

                double x = r_for_perehod * Math.Cos(angle);
                double y = r_for_perehod * Math.Sin(angle);
                double z = 0;
                positions.Add(new Point3D(x, y, z));
            }

            for (int i = 0; i < segments; i++)
            {
                triangleIndices.Add(0);
                triangleIndices.Add(i + 1);
                triangleIndices.Add(i + 2);
            }

            mesh.Positions = positions;
            mesh.TriangleIndices = triangleIndices;

            // Создаем материал и модель
            var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray);
            brush.Opacity = 0.5;
            Material material = new DiffuseMaterial(brush);
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;
            viewport_2d.Children.Add(visual);

            double z_coord_for_camera = 4.959 * b + 0.0943;
            
            camera_2d.Position = new Point3D(0, 0, z_coord_for_camera);
        }
        private void DrawSrez()
        {
            // Делает три среза:
            // 1 - Начало угла, т. е. 0
            // 2 - Конец угла, т. е. Альфа
            // 3 - Выбранный угол

            double R = userInput.R; // Радиус круга
            double b = userInput.b; // Радиус волокна
            double Alpha = userInput.Alpha * 0.01745;
            double angle_step = Math.PI * 2 / segments;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            positions.Add(new Point3D(0, 0, R));
            positions.Add(new Point3D(2*R, 0, R));
            positions.Add(new Point3D(2*R, 0, -R));
            positions.Add(new Point3D(0, 0, -R));

            positions.Add(new Point3D(2 * R * Math.Cos(Alpha), 2 * R * Math.Sin(Alpha), R));
            positions.Add(new Point3D(2 * R * Math.Cos(Alpha), 2 * R * Math.Sin(Alpha), -R));

            triangleIndices.Add(0);
            triangleIndices.Add(1);
            triangleIndices.Add(2);

            triangleIndices.Add(0);
            triangleIndices.Add(2);
            triangleIndices.Add(1);

            triangleIndices.Add(2);
            triangleIndices.Add(3);
            triangleIndices.Add(0);

            triangleIndices.Add(2);
            triangleIndices.Add(0);
            triangleIndices.Add(3);

            triangleIndices.Add(0);
            triangleIndices.Add(3);
            triangleIndices.Add(4);

            triangleIndices.Add(0);
            triangleIndices.Add(4);
            triangleIndices.Add(3);

            triangleIndices.Add(3);
            triangleIndices.Add(4);
            triangleIndices.Add(5);

            triangleIndices.Add(3);
            triangleIndices.Add(5);
            triangleIndices.Add(4);


            mesh.Positions = positions;
            mesh.TriangleIndices = triangleIndices;

            var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.AliceBlue);
            brush.Opacity = 0.5;
            Material material = new DiffuseMaterial(brush);
            
            GeometryModel3D model = new GeometryModel3D(mesh, material);
                
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;
            viewport_3d.Children.Add(visual);
        }
        private void DrawUserSrez(double angle)
        {
            // Делает три среза:
            // 1 - Начало угла, т. е. 0
            // 2 - Конец угла, т. е. Альфа
            // 3 - Выбранный угол

            double R = userInput.R; // Радиус круга
            double user_angle = angle * 0.01745;

            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3DCollection positions = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            Console.WriteLine($"{angle}, {user_angle}");

            positions.Add(new Point3D(0, 0, R));
            positions.Add(new Point3D(0, 0, -R));

            positions.Add(new Point3D(2 * R * Math.Cos(user_angle), 2 * R * Math.Sin(user_angle), R));
            positions.Add(new Point3D(2 * R * Math.Cos(user_angle), 2 * R * Math.Sin(user_angle), -R));

            Console.WriteLine(positions);

            triangleIndices.Add(0);
            triangleIndices.Add(1);
            triangleIndices.Add(2);

            triangleIndices.Add(0);
            triangleIndices.Add(2);
            triangleIndices.Add(1);

            triangleIndices.Add(2);
            triangleIndices.Add(3);
            triangleIndices.Add(1);

            triangleIndices.Add(2);
            triangleIndices.Add(1);
            triangleIndices.Add(3);

            mesh.Positions = positions;
            mesh.TriangleIndices = triangleIndices;

            var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            brush.Opacity = 0.5;

            Material material = new DiffuseMaterial(brush);
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;
            viewport_3d.Children.Add(visual);
        }
        private void CreateAxis()
        {
            var points = new Point3DCollection { 
                // X
                new Point3D(-coef_for_axis * userInput.R, -0.01, 0), // 0
                new Point3D(coef_for_axis * userInput.R, -0.01, 0),  // 1
                new Point3D(coef_for_axis * userInput.R, 0.01, 0),   // 2
                 new Point3D(-coef_for_axis * userInput.R, 0.01, 0), // 3

                // Y
                new Point3D(-0.01, -coef_for_axis * userInput.R, 0), // 4
                new Point3D(0.01, -coef_for_axis * userInput.R, 0),  // 5
                new Point3D(-0.01, coef_for_axis * userInput.R, 0),  // 6
                new Point3D(0.01, coef_for_axis * userInput.R, 0),  // 7

                // Z
                new Point3D(0, -0.01, -coef_for_axis * userInput.R), // 8
                new Point3D(0, 0.01, -coef_for_axis * userInput.R),  // 9
                new Point3D(0, -0.01, coef_for_axis * userInput.R),   // 10
                new Point3D(0, 0.01, coef_for_axis * userInput.R)   // 11
            };

            var indices = new Int32Collection {
                // X
                0, 1, 2,
                0, 2, 1,
                0, 2, 3,
                0, 3, 2,

                // Y
                4, 5, 6,
                4, 6, 5,
                4, 6, 7,
                4, 7, 6,

                // Z
                8, 9, 10,
                8, 10, 9,
                8, 10, 11,
                8, 11, 10,
            };

            var line = new MeshGeometry3D
            {
                Positions = points,
                TriangleIndices = indices
            };

            var material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black));
            var lineModel = new GeometryModel3D(line, material);
            var modelVisual = new ModelVisual3D { Content = lineModel };

            viewport_3d.Children.Add(modelVisual);

            DrawText();
        }
        private void DrawText()
        {
            var p_x = new Point3DCollection
            {
                new Point3D(coef_for_axis * userInput.R - 0.3, 0, 0),
                new Point3D(coef_for_axis * userInput.R, 0, 0),
                new Point3D(coef_for_axis * userInput.R, 0.3, 0),
                new Point3D(coef_for_axis * userInput.R - 0.3, 0.3, 0),
            };

            var p_y = new Point3DCollection
            {
                new Point3D(0, coef_for_axis * userInput.R - 0.3, 0),
                new Point3D(0.3, coef_for_axis * userInput.R - 0.3, 0),
                new Point3D(0.3, coef_for_axis * userInput.R, 0),
                new Point3D(0, coef_for_axis * userInput.R, 0),
            };

            var p_z = new Point3DCollection
            {
                new Point3D(0, 0, coef_for_axis * userInput.R),
                new Point3D(0, 0, coef_for_axis * userInput.R - 0.3),
                new Point3D(0, 0.3, coef_for_axis * userInput.R - 0.3),
                new Point3D(0, 0.3, coef_for_axis * userInput.R),
            };

            CreateText(p_x, "x", viewport_3d);
            CreateText(p_y, "y", viewport_3d);
            CreateText(p_z, "z", viewport_3d);
        }
        private void CreateText(Point3DCollection p, String t, Viewport3D viewport)
        {
            var viewport_for_text = new Viewport2DVisual3D();

            var triangles = new Int32Collection
            {
                0, 1, 2,
                0, 2, 1,
                0, 2, 3,
                0, 3, 2,
            };

            var texture = new PointCollection
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0),
            };

            var meshGeometry = new MeshGeometry3D() { Positions = p, TriangleIndices = triangles, TextureCoordinates = texture };
            var mat = new DiffuseMaterial();
            mat.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);

            var text = new Label() { Content = t };

            viewport_for_text.Visual = text;
            viewport_for_text.Material = mat;
            viewport_for_text.Geometry = meshGeometry;

            viewport.Children.Add(viewport_for_text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CommonMethods.isCanConvertToDouble(Input_Betta.Text))
            {
                int angle_for_2D = int.Parse(Input_Betta.Text);
                if (angle_for_2D <= userInput.Alpha)
                {
                    if (viewport_2d.Children.Count == 8)
                    {
                        viewport_2d.Children.RemoveAt(1);
                        viewport_2d.Children.RemoveAt(1);
                        viewport_2d.Children.RemoveAt(1);
                        viewport_2d.Children.RemoveAt(1);
                        viewport_2d.Children.RemoveAt(1);
                        viewport_2d.Children.RemoveAt(1);
                        viewport_2d.Children.RemoveAt(1);


                        CreateAxis2D();
                        DrawCircle(angle_for_2D);

                        if (viewport_3d.Children.Count == 10)
                        {
                            // TODO:
                            // Сделать создание освещения, через метод
                            // Потом просто очищать всё и вызывать метод
                            
                            viewport_3d.Children.RemoveAt(1);
                            viewport_3d.Children.RemoveAt(1);
                            viewport_3d.Children.RemoveAt(1);
                            viewport_3d.Children.RemoveAt(1);
                            viewport_3d.Children.RemoveAt(1);
                            viewport_3d.Children.RemoveAt(1);
                            viewport_3d.Children.RemoveAt(1);
                            viewport_3d.Children.RemoveAt(1);
                            viewport_3d.Children.RemoveAt(1);

                            DrawCilindr();
                            DrawWire();
                            DrawSrez();
                            CreateAxis();
                            DrawUserSrez(angle_for_2D);
                        }
                    }
                }
            }
        }
    }
}
