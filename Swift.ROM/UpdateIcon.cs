using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Swift.ROM
{
    class UpdateIcon
    {
        delegate void cbUpdateIcon(DataRow row);

        private FormMain formMain;

        public UpdateIcon(FormMain form)
		{
			this.formMain = form;
		}

        public void Start()
        {
            //重写图标
            foreach (DataRow row in this.formMain.dataTableR.Select("i is not null"))
            {
                try
                {
                  //  if (row.IsNull("i")) continue;
                    cbUpdateIcon cb = new cbUpdateIcon(formMain.updateIcon);
                    formMain.listView.Invoke(cb, new object[] { row });
                }
                catch { break; }
            }
        }
    }
}
