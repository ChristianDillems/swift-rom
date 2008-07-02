using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Swift.ROM
{
	public partial class FormPrintPreview : Form
	{
		public FormPrintPreview(DataView dataTable)
		{
			InitializeComponent();

			this.dt = dataTable;
		}

		private DataView dt;

		private void FormPrintPreview_Load(object sender, EventArgs e)
		{
			//this.reportViewer1.LocalReport. = "Report.rdlc";
			//this.reportViewer1.LocalReport.DataSources["DataTableROM"] = this.dt;

			Microsoft.Reporting.WinForms.ReportDataSource ds=new Microsoft.Reporting.WinForms.ReportDataSource("DataSet_DataTableROM",this.dt);

			this.reportViewer1.LocalReport.DataSources.Add(ds);

			this.reportViewer1.RefreshReport();
		}
	}
}