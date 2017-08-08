
namespace Firefly.Wpf.MenuDemo
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using System.Windows.Media.Animation;
	using System.Diagnostics;

	public class SmoothMove: ContentControl
	{
		private TranslateTransform translation = new TranslateTransform();
		private Storyboard animation = new Storyboard();
		private DoubleAnimation animateX;
		private DoubleAnimation animateY;
		private double lastGoingToX = 0;
		private double lastGoingToY = 0;

		public SmoothMove()
		{
			if (Application.Current==null|| !System.ComponentModel.DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow))
			{
				this.LayoutUpdated += this.This_LayoutUpdated;

				this.RenderTransform = this.translation;

				this.animateX = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(.25)));
				Storyboard.SetTargetProperty(this.animateX, new PropertyPath("(0).(1)", new DependencyProperty[] { UIElement.RenderTransformProperty, TranslateTransform.XProperty }));
				animation.Children.Add(this.animateX);

				this.animateY = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(.25)));
				Storyboard.SetTargetProperty(this.animateY, new PropertyPath("(0).(1)", new DependencyProperty[] { UIElement.RenderTransformProperty, TranslateTransform.YProperty }));
				animation.Children.Add(this.animateY);
			}			
		}

		private Visual FindPanel(Visual visual)
		{
			Panel panel = visual as Panel;
			if (panel != null)
			{
				return panel;
			}
			if (visual != null)
			{
				Visual parent = (Visual)VisualTreeHelper.GetParent(visual);
				return this.FindPanel(parent);
			}
			return null;
		}

		private void This_LayoutUpdated(object sender, EventArgs e)
		{
			this.animation.Stop(this);

			Visual panel = this.FindPanel(this);

			if (panel == null)
				return;

			Transform transform = (Transform)this.TransformToAncestor(panel);

			double goingToX = transform.Value.OffsetX - this.translation.X;
			double comingFromX = this.lastGoingToX + this.translation.X;

			this.animateX.From = comingFromX - goingToX;

			this.lastGoingToX = goingToX;

			double goingToY = transform.Value.OffsetY - this.translation.Y;
			double comingFromY = this.lastGoingToY + this.translation.Y;

			this.animateY.From = comingFromY - goingToY;

			this.lastGoingToY = goingToY;

			if (this.animateX.From != 0 || this.animateY.From != 0)
				animation.Begin(this, HandoffBehavior.SnapshotAndReplace, true);
		}
	}
}
