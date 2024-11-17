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

        private int angle_for_2D = 0;

        public ModelOfOV(UserInput uI)
        {
            userInput = uI;
            InitializeComponent();
            this.MouseDown += MouseDownHandler;
            this.MouseMove += MouseMoveHandler;
            this.MouseUp += MouseUpHandler;
            this.MouseLeave += MouseLeaveHandler;
            //CompositionTarget.Rendering += UpdateCamera;
            DrawWire();
            DrawCircle();
            UpdateCamera();
        }

        private void UpdateCamera()
        {

            // Получаем позицию вашего объекта.
            Point3D modelPosition = GetModelPosition(); // Измените на реальную позицию вашей модели
                                                        // Устанавливаем позицию камеры.
            camera_3d.Position = new Point3D(modelPosition.X, modelPosition.Y, modelPosition.Z + 10); // Камера позади объекта
            camera_3d.LookDirection = new Vector3D(modelPosition.X - camera_3d.Position.X,
                                                   modelPosition.Y - camera_3d.Position.Y,
                                                   modelPosition.Z - camera_3d.Position.Z + zoomChange);
        }

        private Point3D GetModelPosition()
        {
            // Предполагаем, что трансформация модели — это TranslateTransform3D
            Model3D model = (GeometryModel3D)((ModelVisual3D)viewport_3d.Children[0]).Content;
            if (model.Transform is Transform3DGroup transformGroup)
            {
                // Применяем трансформацию к начальной позиции
                Point3D position = new Point3D(0, 0, 0);
                foreach (var transform in transformGroup.Children)
                {
                    if (transform is TranslateTransform3D translate)
                    {
                        position.X += translate.OffsetX;
                        position.Y += translate.OffsetY;
                        position.Z += translate.OffsetZ;
                    }
                }
                return position;
            }

            // Если моделям не задана трансформация, возвращаем 0,0,0
            return new Point3D(0, 0, 0);
        }

        private double zoomChange = 0;
        private const double ZoomFactor = 0.5;
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Изменяем позицию камеры в зависимости от направления прокрутки колеса мыши
            zoomChange = e.Delta > 0 ? -ZoomFactor : ZoomFactor;
            camera_3d.Position = new Point3D(
                camera_3d.Position.X,
                camera_3d.Position.Y,
                camera_3d.Position.Z + zoomChange
            );
            // Принуждаем камеру смотреть на центр объекта (если нужно)

            // myCamera.LookDirection = new Vector3D(0, 0, -1); // или другое направление

        }

        private bool isDragging;
        private Point lastMousePosition;
        private void MouseDownHandler(object sender, MouseButtonEventArgs e)    
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                isDragging = true;
                lastMousePosition = e.GetPosition(this);
                this.CaptureMouse();
            }

        }
        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentMousePosition = e.GetPosition(this);
                Vector delta = currentMousePosition - lastMousePosition;

                var rotationY = new AxisAngleRotation3D(new Vector3D(0, 1, 0), delta.X * 0.5);
                var rotationX = new AxisAngleRotation3D(new Vector3D(1, 0, 0), delta.Y * 0.5);

                var transformGroup = new Transform3DGroup();

                transformGroup.Children.Add(new RotateTransform3D(rotationY));
                transformGroup.Children.Add(new RotateTransform3D(rotationX));

                var model = (GeometryModel3D)((ModelVisual3D)viewport_3d.Children[0]).Content;
                var transform = model.Transform as Transform3DGroup;

                if (transform != null) transform.Children.Add(transformGroup);
                else model.Transform = transformGroup;
                
                lastMousePosition = currentMousePosition;
            }
        }
        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            this.ReleaseMouseCapture();
        }
        private void MouseLeaveHandler(object sender, MouseEventArgs e)
        {
            isDragging = false;
            this.ReleaseMouseCapture();
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

            double Alpha_angle = userInput.Alpha;
            double h = 0.1;
            double dlina_wire = 100;
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


            double h_for_perehoda = Math.PI / (Alpha_angle * 0.01745);
            for (int i = 0; i <= (dlina / h); i++)
            {
                if ((i >= 0 && i <= dlina_prodolzhenie / h) || (i >= (dlina_wire + dlina_prodolzhenie) / h))
                {
                    int lyam = i < (dlina_prodolzhenie + dlina_wire) / h ? lyam = -1 : lyam = 1;

                    tmp_rot_x += Math.Sin(lyam * angle_for_rotation - 1.5708) * h;
                    tmp_rot_y -= Math.Cos(lyam * angle_for_rotation - 1.5708) * h;
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
            
            visual.Content = model;
            Console.WriteLine(-visual.Content.Bounds.X);
            Console.WriteLine(-visual.Content.Bounds.Y);
            Console.WriteLine(-visual.Content.Bounds.Z);

            //TranslateTransform3D translateTransform = new TranslateTransform3D(-visual.Content.Bounds.X, -visual.Content.Bounds.Y, -visual.Content.Bounds.Z);
            visual.Transform = transform;

            viewport_3d.Children.Add(visual);
        }


        private void DrawCircle()
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
            Material material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue));
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            // Добавляем модель в ModelVisual3D
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = model;
            viewport_2d.Children.Add(visual);

            double z_coord_for_camera = 4.959 * b + 0.0943;
            
            camera_2d.Position = new Point3D(0, 0, z_coord_for_camera);
        }

        private void Input_Betta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CommonMethods.isCanConvertToDouble(Input_Betta.Text))
            {
                angle_for_2D = int.Parse(Input_Betta.Text);
            }

            if (angle_for_2D <= userInput.Alpha)
            {
                if (viewport_2d != null)
                {
                    viewport_2d.Children.Clear();
                    DrawCircle();
                }
            }
        }
    }
}
