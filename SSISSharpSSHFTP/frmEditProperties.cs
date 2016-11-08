using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using SSISSFTPTask110.SSIS;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DataTransformationServices.Controls;
using Microsoft.SqlServer.Dts.Runtime;
using TaskHost = Microsoft.SqlServer.Dts.Runtime.TaskHost;
using Variable = Microsoft.SqlServer.Dts.Runtime.Variable;

namespace SSISSFTPTask110
{
    public partial class frmEditProperties : Form
    {
        #region Private Properties
        private readonly TaskHost _taskHost;
        private readonly Connections _connections;
        private readonly Variables _variables;
        #endregion

        #region .ctor
        public frmEditProperties(TaskHost taskHost, Connections connections)
        {
            InitializeComponent();

            _taskHost = taskHost;
            _connections = connections;
            _variables = taskHost.Variables;

            if (taskHost == null)
            {
                throw new ArgumentNullException("taskHost");
            }

            FillConnectionInfoPanel();
            FillDetailsPanel();
            FillRecordSetElements();
            GetAssemblyVersion();
        }

        private void GetAssemblyVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            linkLabelCodeplex.Text = string.Format("v. {0}", version);
        }



        #endregion

        #region Events

        private void optPublicKeyFileConnection_Click(object sender, EventArgs e)
        {
            LoadKeyFileConnections();
        }

        private void optPublicKeyFileVariable_Click(object sender, EventArgs e)
        {
            cmbKeyFile.Items.Clear();

            cmbKeyFile.Items.Add(string.Empty);

            if (_taskHost.Properties[Keys.PRIVATE_KEY_FILE_FROM_FILE_CONNECTION].GetValue(_taskHost) != null)
            {
                if (_taskHost.Properties[Keys.PRIVATE_KEY_FILE_FROM_FILE_CONNECTION].GetValue(_taskHost).ToString() == Keys.FALSE)
                {
                    if (_taskHost.Properties[Keys.PRIVATE_KEY_FILE].GetValue(_taskHost) != null)
                    {
                        cmbKeyFile.Items.Add(_taskHost.Properties[Keys.PRIVATE_KEY_FILE].GetValue(_taskHost).ToString());
                    }
                }
            }

            cmbKeyFile.Items.AddRange((from Variable var in _variables
                                       where var.DataType == Type.GetTypeCode(Type.GetType("System.String")) &&
                                             var.Namespace.ToLower() == "user"
                                       select string.Format("@[{0}::{1}]", var.Namespace, var.Name)).ToArray());

        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            //Save the values
            _taskHost.Properties[Keys.FTP_SERVER].SetValue(_taskHost, cmbServer.Text);
            _taskHost.Properties[Keys.FTP_USER].SetValue(_taskHost, cmbUser.Text);
            _taskHost.Properties[Keys.FTP_PASSWORD].SetValue(_taskHost, cmbPassword.Text);

            _taskHost.Properties[Keys.FTP_PORT].SetValue(_taskHost, !string.IsNullOrEmpty(cmbPort.Text) ? cmbPort.Text : "22");

            _taskHost.Properties[Keys.ENCRYPTION_TYPE].SetValue(_taskHost, optionEncryptionPassword.Checked ? Keys.ENCRYPTION_TYPE_PASSWORD : Keys.ENCRYPTION_TYPE_KEY);
            _taskHost.Properties[Keys.PRIVATE_KEY_FILE_FROM_FILE_CONNECTION].SetValue(_taskHost, optPublicKeyFileConnection.Checked ? Keys.TRUE : Keys.FALSE);
            _taskHost.Properties[Keys.PRIVATE_KEY_FILE].SetValue(_taskHost, cmbKeyFile.Text);
            _taskHost.Properties[Keys.PASS_PHRASE].SetValue(_taskHost, cmbPassPhrase.Text);

            _taskHost.Properties[Keys.FTP_ACTION_LIST].SetValue(_taskHost, cmbAction.SelectedItem);

            _taskHost.Properties[Keys.FTP_LOCAL_PATH].SetValue(_taskHost, cmbLocal.Text);
            _taskHost.Properties[Keys.FTP_LOCAL_PATH_SOURCE_TYPE].SetValue(_taskHost, optFileConnection.Checked ? Keys.TRUE : Keys.FALSE);
            _taskHost.Properties[Keys.FTP_LOCAL_PATH_OVERWRITE].SetValue(_taskHost, chkOverwrite.Checked ? Keys.TRUE : Keys.FALSE);

            _taskHost.Properties[Keys.FTP_REMOTE_PATH].SetValue(_taskHost, cmbRemote.Text);

            _taskHost.Properties[Keys.FTP_FILES_LIST].SetValue(_taskHost, cmbFilesList.Visible ? cmbFilesList.SelectedItem ?? string.Empty : string.Empty);

            _taskHost.Properties[Keys.SLEEP_ON_DISCONNECT].SetValue(_taskHost, chkSleep.Checked ? Keys.TRUE : Keys.FALSE);
            _taskHost.Properties[Keys.SLEEP_SECONDS].SetValue(_taskHost, numericUpDown.Text);

            //Recordset
            _taskHost.Properties[Keys.RecordsetEnabled].SetValue(_taskHost, chkResultSetEnabled.Checked);
            _taskHost.Properties[Keys.RecordsetColumnIndex].SetValue(_taskHost, Int32.Parse(domainUpDownIndex.Text));
            _taskHost.Properties[Keys.RecordsetVariable].SetValue(_taskHost, cmbRecordset.Text);
            _taskHost.Properties[Keys.ValueIsFullPath].SetValue(_taskHost, optFullPath.Checked);

            _taskHost.Properties[Keys.DeleteFileOnTransferCompleted].SetValue(_taskHost, chkDeleteFileOnTransferCompleted.Checked ? Keys.TRUE : Keys.FALSE);

            _taskHost.Properties[Keys.RecursiveCopy].SetValue(_taskHost, chkRecursive.Checked);

            int depth = 1;
            Int32.TryParse(txDepth.Text, out depth);

            _taskHost.Properties[Keys.RecursiveCopyDepth].SetValue(_taskHost, depth);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void chkSleep_Click(object sender, EventArgs e)
        {
            numericUpDown.Enabled = chkSleep.Checked;
        }

        private void btSourceFile_Click(object sender, EventArgs e)
        {
            using (ExpressionBuilder expressionBuilder = ExpressionBuilder.Instantiate(_variables, _taskHost.VariableDispenser, typeof(string), cmbLocal.Text))
            {
                if (expressionBuilder.ShowDialog() == DialogResult.OK)
                {
                    cmbLocal.Text = expressionBuilder.Expression;
                }
            }
        }

        private void btKeyFileExpression_Click(object sender, EventArgs e)
        {
            using (ExpressionBuilder expressionBuilder = ExpressionBuilder.Instantiate(_variables, _taskHost.VariableDispenser, typeof(string), cmbKeyFile.Text))
            {
                if (expressionBuilder.ShowDialog() == DialogResult.OK)
                {
                    cmbKeyFile.Text = expressionBuilder.Expression;
                }
            }
        }

        private void btDestinationFile_Click(object sender, EventArgs e)
        {
            using (ExpressionBuilder expressionBuilder = ExpressionBuilder.Instantiate(_variables, _taskHost.VariableDispenser, typeof(string), cmbRemote.Text))
            {
                if (expressionBuilder.ShowDialog() == DialogResult.OK)
                {
                    cmbRemote.Text = expressionBuilder.Expression;
                }
            }
        }

        private void optFileConnection_Click(object sender, EventArgs e)
        {
            if (optFileConnection.Checked)
            {
                btSourceFile.Enabled = false;
                LoadLocalFileConnections();
            }
            else
            {
                btSourceFile.Enabled = true;
                LoadLocalVariables();
            }
        }

        private void optFileVariable_Click(object sender, EventArgs e)
        {
            if (optFileConnection.Checked)
            {
                btSourceFile.Enabled = false;
                LoadLocalFileConnections();
            }
            else
            {
                btSourceFile.Enabled = true;
                LoadLocalVariables();
            }
        }

        private void linkLabelCodeplex_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://ssissftp.codeplex.com/releases/");
        }

        private void cmbAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAction.Text == Communication.ActionTask[8])
            {
                cmbFilesList.Visible = label1.Visible = true;
            }
            else
            {
                cmbFilesList.Visible = label1.Visible = false;
            }
        }

        private void optionEncryptionKey_Click(object sender, EventArgs e)
        {
            groupBoxEncryption.Enabled = true;
        }

        private void optionEncryptionPassword_Click(object sender, EventArgs e)
        {
            groupBoxEncryption.Enabled = false;
        }

        private void chkResultSetEnabled_Click(object sender, EventArgs e)
        {
            EnableRecordsetControls(chkResultSetEnabled.Checked);
        }

        private void chkRecursive_Click(object sender, EventArgs e)
        {
            txDepth.Enabled = chkRecursive.Checked;
        }

        #endregion

        #region Methods

        private List<string> LoadUserVariables(string parameterInfo)
        {
            return (from Variable var in _variables
                    where var.DataType == Type.GetTypeCode(Type.GetType(parameterInfo)) && var.Namespace.ToLower() == "user"
                    select string.Format("@[{0}::{1}]", var.Namespace, var.Name)).ToList();
        }

        private void LoadLocalVariables()
        {
            cmbLocal.Items.Clear();
            cmbLocal.Items.AddRange(LoadUserVariables("System.String").ToArray());
        }

        private void LoadRecordSetVariables()
        {
            cmbRecordset.Items.Clear();
            cmbRecordset.Items.AddRange(LoadUserVariables("System.Object").ToArray());
        }

        private void LoadRemoteVariables()
        {
            cmbRemote.Items.Clear();
            cmbRemote.Items.AddRange(LoadUserVariables("System.String").ToArray());
        }

        private void LoadLocalFileConnections()
        {
            cmbLocal.Items.Clear();
            foreach (var connection in _connections.Cast<ConnectionManager>().Where(connection => connection.CreationName == "FILE"))
            {
                cmbLocal.Items.Add(connection.Name);
            }
        }

        private void LoadKeyFileConnections()
        {
            cmbKeyFile.Items.Clear();
            foreach (var connection in _connections.Cast<ConnectionManager>().Where(connection => connection.CreationName == "FILE"))
            {
                cmbKeyFile.Items.Add(connection.Name);
            }
        }

        private void FillConnectionInfoPanel()
        {
            try
            {
                foreach (var variable in (from Variable var in _variables
                                          where var.DataType == Type.GetTypeCode(Type.GetType("System.String")) &&
                                                var.Namespace.ToLower() == "user"
                                          select var))
                {
                    cmbServer.Items.Add(string.Format("@[{0}::{1}]", variable.Namespace, variable.Name));
                    cmbUser.Items.Add(string.Format("@[{0}::{1}]", variable.Namespace, variable.Name));
                    cmbPassword.Items.Add(string.Format("@[{0}::{1}]", variable.Namespace, variable.Name));
                    cmbPassPhrase.Items.Add(string.Format("@[{0}::{1}]", variable.Namespace, variable.Name));
                }

                foreach (var variable in (from Variable var in _variables
                                          where (var.DataType == Type.GetTypeCode(Type.GetType("System.Int32")) ||
                                                 var.DataType == Type.GetTypeCode(Type.GetType("System.Int16"))) &&
                                                 var.Namespace.ToLower() == "user"
                                          select var))
                {
                    cmbPort.Items.Add(string.Format("@[{0}::{1}]", variable.Namespace, variable.Name));
                }

                cmbServer.SelectedIndex = GetSelectedComboBoxIndex(cmbServer, _taskHost.Properties[Keys.FTP_SERVER].GetValue(_taskHost));
                cmbUser.SelectedIndex = GetSelectedComboBoxIndex(cmbUser, _taskHost.Properties[Keys.FTP_USER].GetValue(_taskHost));
                cmbPassword.SelectedIndex = GetSelectedComboBoxIndex(cmbPassword, _taskHost.Properties[Keys.FTP_PASSWORD].GetValue(_taskHost));
                cmbPort.SelectedIndex = GetSelectedComboBoxIndex(cmbPort, _taskHost.Properties[Keys.FTP_PORT].GetValue(_taskHost));

                if (_taskHost.Properties[Keys.ENCRYPTION_TYPE].GetValue(_taskHost) != null)
                {
                    if (_taskHost.Properties[Keys.ENCRYPTION_TYPE].GetValue(_taskHost).ToString() == Keys.ENCRYPTION_TYPE_PASSWORD)
                    {
                        optionEncryptionPassword.Checked = true;
                        groupBoxEncryption.Enabled = false;
                    }
                    else
                    {
                        optionEncryptionKey.Checked = true;
                        groupBoxEncryption.Enabled = true;
                    }

                    if (_taskHost.Properties[Keys.PRIVATE_KEY_FILE_FROM_FILE_CONNECTION].GetValue(_taskHost) != null)
                    {
                        if (_taskHost.Properties[Keys.PRIVATE_KEY_FILE_FROM_FILE_CONNECTION].GetValue(_taskHost).ToString() == Keys.TRUE)
                        {
                            optPublicKeyFileConnection.Checked = true;
                            LoadKeyFileConnections();
                        }
                        else
                        {
                            optPublicKeyFileVariable.Checked = true;
                            LoadFileKeyPathVariables();
                        }
                    }

                    cmbKeyFile.SelectedIndex = GetSelectedComboBoxIndex(cmbKeyFile, _taskHost.Properties[Keys.PRIVATE_KEY_FILE].GetValue(_taskHost));
                    cmbPassPhrase.SelectedIndex = GetSelectedComboBoxIndex(cmbPassPhrase, _taskHost.Properties[Keys.PASS_PHRASE].GetValue(_taskHost));
                }
                else
                {
                    optionEncryptionPassword.Checked = true;
                    groupBoxEncryption.Enabled = false;
                    LoadKeyFileConnections();
                }

            }
            catch (Exception exception)
            {

            }
        }

        private void LoadFileKeyPathVariables()
        {
            cmbKeyFile.Items.Clear();
            cmbKeyFile.Items.AddRange((from Variable var in _variables
                                       where var.DataType == Type.GetTypeCode(Type.GetType("System.String")) &&
                                             var.Namespace.ToLower() == "user"
                                       select string.Format("@[{0}::{1}]", var.Namespace, var.Name)).ToArray());
        }

        private void SwitchEncryptionTypePassword(bool type)
        {
            cmbUser.Enabled = cmbUser.Enabled = type;
            cmbKeyFile.Enabled = btKeyFileExpression.Enabled = optPublicKeyFileConnection.Enabled = optPublicKeyFileVariable.Enabled = cmbPassPhrase.Enabled = !type;
        }

        private void FillRecordSetElements()
        {
            if (_taskHost.Properties[Keys.RecordsetEnabled].GetValue(_taskHost) != null)
            {
                chkResultSetEnabled.Checked = (bool)_taskHost.Properties[Keys.RecordsetEnabled].GetValue(_taskHost);
                EnableRecordsetControls(chkResultSetEnabled.Checked);

                if (_taskHost.Properties[Keys.RecordsetColumnIndex].GetValue(_taskHost) != null)
                {
                    domainUpDownIndex.Text = _taskHost.Properties[Keys.RecordsetColumnIndex].GetValue(_taskHost).ToString();
                }

                if (_taskHost.Properties[Keys.ValueIsFullPath].GetValue(_taskHost) != null)
                {
                    if ((bool)_taskHost.Properties[Keys.ValueIsFullPath].GetValue(_taskHost))
                    {
                        optFullPath.Checked = true;
                        optNameOnly.Checked = !optFullPath.Checked;
                    }
                    else
                    {
                        optNameOnly.Checked = true;
                        optFullPath.Checked = !optNameOnly.Checked;
                    }
                }

                LoadRecordSetVariables();
                cmbRecordset.SelectedIndex = GetSelectedComboBoxIndex(cmbRecordset, _taskHost.Properties[Keys.RecordsetVariable].GetValue(_taskHost));
            }
            else
            {
                chkSleep.Checked = false;
                LoadRecordSetVariables();
                EnableRecordsetControls(false);
            }
        }

        private void EnableRecordsetControls(bool enable)
        {
            label8.Enabled = enable;
            cmbRecordset.Enabled = enable;
            label9.Enabled = enable;
            domainUpDownIndex.Enabled = enable;
            label10.Enabled = enable;
            optNameOnly.Enabled = enable;
            optFullPath.Enabled = enable;
        }

        private void FillDetailsPanel()
        {
            try
            {
                cmbAction.Items.AddRange(Communication.ActionTask.ToArray());

                if (_taskHost.Properties[Keys.FTP_ACTION_LIST].GetValue(_taskHost) != null)
                    cmbAction.SelectedIndex = cmbAction.FindString(_taskHost.Properties[Keys.FTP_ACTION_LIST].GetValue(_taskHost).ToString());

                if (_taskHost.Properties[Keys.FTP_LOCAL_PATH_SOURCE_TYPE].GetValue(_taskHost) != null)
                {
                    if (_taskHost.Properties[Keys.FTP_LOCAL_PATH_SOURCE_TYPE].GetValue(_taskHost).ToString() == Keys.TRUE)
                    {
                        optFileConnection.Checked = true;
                        btSourceFile.Enabled = false;
                        LoadLocalFileConnections();
                    }
                    else
                    {
                        btSourceFile.Enabled = true;
                        optFileVariable.Checked = true;
                        LoadLocalVariables();
                    }
                }
                else
                {
                    btSourceFile.Enabled = false;
                    optFileConnection.Checked = true;
                    LoadLocalFileConnections();
                }

                if (_taskHost.Properties[Keys.FTP_LOCAL_PATH_OVERWRITE].GetValue(_taskHost) != null)
                {
                    chkOverwrite.Checked = _taskHost.Properties[Keys.FTP_LOCAL_PATH_OVERWRITE].GetValue(_taskHost).ToString() == Keys.TRUE;
                }
                else
                {
                    chkOverwrite.Checked = true;
                }


                if (_taskHost.Properties[Keys.DeleteFileOnTransferCompleted].GetValue(_taskHost) != null)
                {
                    chkDeleteFileOnTransferCompleted.Checked = _taskHost.Properties[Keys.DeleteFileOnTransferCompleted].GetValue(_taskHost).ToString() == Keys.TRUE;
                }
                else
                {
                    chkDeleteFileOnTransferCompleted.Checked = false;
                }



                LoadRemoteVariables();

                foreach (var variable in (from Variable var in _variables
                                          where var.DataType == Type.GetTypeCode(typeof(object)) &&
                                                var.Namespace.ToLower() == "user"
                                          select var))
                {
                    cmbFilesList.Items.Add(string.Format("@[{0}::{1}]", variable.Namespace, variable.Name));
                }

                cmbFilesList.SelectedIndex = GetSelectedComboBoxIndex(cmbFilesList, _taskHost.Properties[Keys.FTP_FILES_LIST].GetValue(_taskHost));

                cmbLocal.SelectedIndex = GetSelectedComboBoxIndex(cmbLocal, _taskHost.Properties[Keys.FTP_LOCAL_PATH].GetValue(_taskHost));

                cmbRemote.SelectedIndex = GetSelectedComboBoxIndex(cmbRemote, _taskHost.Properties[Keys.FTP_REMOTE_PATH].GetValue(_taskHost));

                if (_taskHost.Properties[Keys.SLEEP_ON_DISCONNECT].GetValue(_taskHost) != null)
                {
                    chkSleep.Checked = _taskHost.Properties[Keys.SLEEP_ON_DISCONNECT].GetValue(_taskHost).ToString() == Keys.TRUE;
                    numericUpDown.Enabled = chkSleep.Checked;
                    if (_taskHost.Properties[Keys.SLEEP_SECONDS].GetValue(_taskHost) != null)
                    {
                        Int32 seconds = 0;
                        numericUpDown.Value = Int32.TryParse(_taskHost.Properties[Keys.SLEEP_SECONDS].GetValue(_taskHost).ToString(),
                                                             NumberStyles.Integer,
                                                             CultureInfo.CreateSpecificCulture("en-GB"),
                                                             out seconds)
                                                    ? seconds
                                                    : 0;
                    }
                }
                else
                {
                    chkOverwrite.Checked = true;
                }

                if (_taskHost.Properties[Keys.RecursiveCopy].GetValue(_taskHost) != null)
                {
                    chkRecursive.Checked = (bool)_taskHost.Properties[Keys.RecursiveCopy].GetValue(_taskHost);
                }
                else
                {
                    chkRecursive.Checked = false;
                }

                if (_taskHost.Properties[Keys.RecursiveCopyDepth].GetValue(_taskHost) != null)
                {
                    Int32 depth = 0;

                    txDepth.Value = Int32.TryParse(_taskHost.Properties[Keys.RecursiveCopyDepth].GetValue(_taskHost).ToString(),
                                                             NumberStyles.Integer,
                                                             CultureInfo.CreateSpecificCulture("en-GB"),
                                                             out depth)
                                                    ? depth
                                                    : 0;
                }

            }
            catch
            {

            }
        }

        private static int GetSelectedComboBoxIndex(ComboBox comboBox, object value)
        {
            int retValue = -1;

            if (value == null)
                return retValue;

            if (string.IsNullOrEmpty(value.ToString()))
                return retValue;

            string strValue = value.ToString();

            if (comboBox.FindString(strValue) > -1)
            {
                retValue = comboBox.FindString(strValue);
            }
            else
            {
                comboBox.Items.Add(strValue);
                retValue = comboBox.FindString(strValue);
            }

            return retValue;
        }

        #endregion
    }
}
