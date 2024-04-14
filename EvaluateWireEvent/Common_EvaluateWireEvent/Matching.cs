using Eto.Drawing;
using Eto.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EvaluateWireEvent
{
    public static partial class Utilities
    {
        public static void CheckMatching(this IGH_Param param)
        {
            if (param.Sources.LastOrDefault() is IGH_Param param_lastAdded)
            {
                PathChecker pathChecker = new PathChecker(param_lastAdded, param);
                if (pathChecker.IsRemarkableMatch)
                    pathChecker.Show();
            }
        }
    }

    /// <summary>
    /// Checks path in wiring, just showing information.
    /// </summary>
    internal partial class PathChecker : Form
    {
        /// <summary>
        /// Left parameter in wiring.
        /// </summary>
        private IGH_Param m_left;
        /// <summary>
        /// Right parameter in wiring.
        /// </summary>
        private IGH_Param m_right;

        /// <summary>
        /// Difference of path-counts between the left param and the right param, to make this form pop-up.
        /// </summary>
        private int m_tolerance_PathCountDiff = 100;

        /// <summary>
        /// If true, the wiring is remarkable.
        /// </summary>
        public bool IsRemarkableMatch
        {
            get
            {
                if (m_left == null || m_right == null)
                    return false;
                else
                    return Math.Abs(m_right.VolatileData.PathCount - m_left.VolatileData.PathCount) > m_tolerance_PathCountDiff;
            }
        }

        /// <summary>
        /// Construct a form instance.
        /// </summary>
        /// <param name="left">Left parameter in wiring.</param>
        /// <param name="right">Right parameter in wiring.</param>
        public PathChecker(IGH_Param left, IGH_Param right)
        {
            m_left = left;
            m_right = right;
        }

        /// <summary>
        /// Construct this layout.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            var pt = GH_Convert.ToPoint(Grasshopper.Instances.ActiveCanvas.Viewport.ProjectPoint(m_left.Attributes.InputGrip));

            Title = "Remark Path Match";
            AutoSize = true;
            Resizable = false;
            Padding = 15;
            Topmost = true;
            Location = new Point(pt.X, pt.Y);

            Button close = new Button { Text = "Close" };
            close.Click += (sender, arg) => Close();

            GH_Structure<IGH_Goo> structure_right = new GH_Structure<IGH_Goo>();
            foreach(GH_Path path in m_right.VolatileData.Paths)
            {
                if (!m_left.VolatileData.Paths.Contains(path))
                {
                    if (m_right.VolatileData.get_Branch(path) is List<IGH_Goo> branch)
                    {
                        structure_right.Paths.Add(path);
                        structure_right.Branches.Add(branch);
                    }
                }
                else
                {
                    IList br_right = m_right.VolatileData.get_Branch(path);
                    IList br_left = m_left.VolatileData.get_Branch(path);
                    if (br_right.Count == br_left.Count)
                        continue;
                    else
                    {
                        structure_right.Paths.Add(path);
                        //structure_right.Branches.Add(br_right.Except(br_left).ToList());
                    }
                }
            }

            DynamicLayout layout = new DynamicLayout { Spacing = new Size(20, 5) };

            layout.AddSeparateRow(new Label
            {
                Text = "Too much difference in matching data trees.",
                TextAlignment = TextAlignment.Center,
            });
            layout.AddSeparateRow(null);
            layout.AddSeparateRow(new ImageView { Image = DrawImage(m_left, m_right).ToEto(), Width = 500, Height = 300 });
            layout.AddSeparateRow(null);
            layout.AddSeparateRow(m_right.PathDescription(structure_right));
            layout.AddSeparateRow(null);
            layout.AddSeparateRow(new Rhino.UI.Controls.Divider());
            layout.AddSeparateRow(null);
            layout.AddSeparateRow(m_left.PathDescription());
            layout.AddSeparateRow(null);
            layout.AddSeparateRow(null, close, null);

            Content = layout;

            base.OnShown(e);
        }
    }

    internal static class DescribeDataTree
    {
        /// <summary>
        ///  The helper function to make description of a param path 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="structure"></param>
        public static DynamicLayout PathDescription(this IGH_Param param, IGH_Structure structure = null)
        {
            DynamicLayout layout = new DynamicLayout();
            IGH_Structure str = structure == null ? param.VolatileData : structure;

            layout.DescribeTopLevel(param);
            layout.DescribeBranches(str);
            layout.DescribePathCount(str);

            return layout;
        }

        private static void DescribeTopLevel(this DynamicLayout layout, IGH_Param param)
        {
            Label paramname = new Label
            {
                Text = param.Name + (param.GetParentComponent() == null ? "" : $" ({param.GetParentComponent().Name})"),
                Font = new Font(SystemFont.Bold)
            };
            Label access = new Label
            {
                Text = $" Access : {param.Access}",
                TextColor = Color.Parse("Gray"),
                Font = new Font(SystemFont.Bold),
            };
            layout.AddSeparateRow(paramname, null, access);
        }

        private static void DescribePathCount(this DynamicLayout layout, IGH_Structure structure)
        {
            Label pathcount = new Label
            {
                Text = $"Path Count : {structure.PathCount}",
                Font = new Font(SystemFont.Bold),
                TextColor = Color.Parse("Gray")
            };
            layout.AddSeparateRow(null, pathcount);
        }

        private static void DescribeBranches(this DynamicLayout layout, IGH_Structure structure)
        {
            int counter = 0;
            foreach (GH_Path path in structure.Paths)
            {
                bool stopper = false;
                if (counter < 6)
                {
                    IList list = structure.get_Branch(path);

                    Label pathLabel = new Label { Text = $"\t{path.ToString(true)}" };
                    Label listCountLabel = new Label { Text = $"List Count : {list.Count}", TextColor = Color.Parse("Gray") };
                    layout.AddSeparateRow(pathLabel, null, listCountLabel);

                    Label itemsLabel = new Label { Text = "\t\t" };
                    if (list.Count == 0)
                    {
                        itemsLabel.Text += "No items";
                        itemsLabel.TextColor = Color.Parse("Red");
                    }
                    else
                    {
                        itemsLabel.TextColor = Color.Parse("Gray");
                        foreach (var item in list)
                        {
                            if (item is Grasshopper.Kernel.Types.IGH_Goo goo)
                                itemsLabel.Text += $"{goo.TypeName}, ";
                            if (itemsLabel.Text.Count() >60)
                            {
                                itemsLabel.Text += ".....";
                                break;
                            }
                        }
                        itemsLabel.Text = itemsLabel.Text.Substring(0, itemsLabel.Text.Length - 2);
                    }
                    layout.AddSeparateColumn(itemsLabel);
                }
                else
                    stopper = true;

                if (stopper)
                {
                    layout.AddSeparateColumn(new Label { Text = "..." });
                    break;
                }
                counter++;
            }
        }
    }
}
