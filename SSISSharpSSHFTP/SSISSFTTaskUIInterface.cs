﻿using System;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace SSISSFTPTask110
{
    class SSISSFTTaskUIInterface : IDtsTaskUI
    {
        #region Private Variables

        private TaskHost _taskHost;
        private Connections _connections;
        #endregion

        #region Constructor
        public SSISSFTTaskUIInterface()
        {
        }

        #endregion

        #region IDtsTaskUI Interface

        public void Initialize(TaskHost taskHost, IServiceProvider serviceProvider)
        {
            _taskHost = taskHost;
            IDtsConnectionService cs = serviceProvider.GetService(typeof(IDtsConnectionService)) as IDtsConnectionService;
            _connections = cs.GetConnections();
        }

        /// <summary>
        /// Show the property window
        /// </summary>
        /// <returns></returns>
        public ContainerControl GetView()
        {
            return new frmEditProperties(_taskHost, _connections);
        }

        public void Delete(IWin32Window parentWindow)
        {
        }

        public void New(IWin32Window parentWindow)
        {
        }

        #endregion
    }
}
