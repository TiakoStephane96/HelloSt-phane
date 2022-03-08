using HelloPam.BLL;
using HelloStéphane.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HelloStéphane.Winforms
{
    public partial class FrmUserEdit : Form
    {
        private readonly Action<IEnumerable<User>> callback;
        private readonly User user;
        private readonly UserBLO userBLO;

        public FrmUserEdit()
        {
            InitializeComponent();
            userBLO = new UserBLO();
            InitForm();
        }

        public FrmUserEdit(Action<IEnumerable<User>> callback):this()
        {
            this.callback = callback;
        }

        public FrmUserEdit(Action<IEnumerable<User>> callback, User user):this(callback)
        {
            this.user = user;
            txtfullname.Text = user.Fullname;
            txtPassword.Text = user.Password;
            txtUsername.Text = user.Username;
            cbxProfile.SelectedIndex = (int)user.Profile;
            chbStatus.Checked = user.Status ?? false;
            if(user.Picture != null)
            pbPicture.Image = Image.FromStream(new MemoryStream(user.Picture)) ;
        }

        private void InitForm()
        {
            txtfullname.Clear();
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Clear();
            llbShowHidePassword.Text = "Show";
            txtUsername.Clear();
            cbxProfile.DataSource = Enum.GetNames(typeof(User.ProfileOptions));
            cbxProfile.SelectedIndex = -1;
            pbPicture.Image = null;
            chbStatus.Checked = true;
            chbStatus.Text = "Enable";
            pbPicture.ImageLocation = null;
            txtfullname.Focus();
        }

        private void validateForm()
        {
            string error = string.Empty;
            if (string.IsNullOrWhiteSpace(txtfullname.Text))
                error += "Full name cannot be empty !\n";
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
                error += "Username cannot be empty !\n";
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
                error += "Password cannot be empty !\n";
            if (cbxProfile.SelectedIndex < 0)
                error += "You must choose a profile !\n";

            if (!string.IsNullOrEmpty(error))
                throw new InvalidExpressionException(error);
        }

        private void llbShowHidePassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            llbShowHidePassword.Text = llbShowHidePassword.Text == "Show" ? "Hide" : "Show";
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
        }

        private void chbStatus_CheckedChanged(object sender, EventArgs e)
        {
            chbStatus.Text = chbStatus.Checked ? "Enable" : "Disable";
        }

        private void pbPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.gif;*.tiff";

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pbPicture.ImageLocation = openFileDialog.FileName;
            }
        }

        private void llbRemovePicture_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pbPicture.ImageLocation = null;
            pbPicture.Image = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                validateForm();

                byte[] picture = this.user?.Picture;
                if(!string.IsNullOrEmpty(pbPicture.ImageLocation) &&
                    File.Exists(pbPicture.ImageLocation))
                {
                    picture = File.ReadAllBytes(pbPicture.ImageLocation);
                }

                User newUser = new User
                (
                    this.user?.Id ?? 0,
                    txtUsername.Text,
                    txtPassword.Text,
                    txtfullname.Text,
                    (User.ProfileOptions)cbxProfile.SelectedIndex,
                    chbStatus.Checked,
                    picture,
                    DateTime.Now

                ) ;

                if (this.user == null)
                {
                    userBLO.CreateUser(newUser);
                }
                else
                {
                    userBLO.EditUser(newUser);
                }

                MessageBox.Show
                    (
                        "Save done !",
                        "Confirmation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                if (this.callback != null)
                    callback(userBLO.FindUser());

                if (this.user != null)
                    this.Close();

                InitForm();
            }
            catch(InvalidExpressionException ex)
            {
                MessageBox.Show
                    (
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show
                    (
                        "An error occured. please try again later !",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                Console.Error.WriteLine($"---> {ex.Message}");
            }
        }

        private void FrmUserEdit_Load(object sender, EventArgs e)
        {

        }
    }
}
