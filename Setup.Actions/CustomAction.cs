using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Setup.Actions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult ExtractDependencies(Session session)
        {            
            session.Log("Begin ExtractDependencies");

            string installPath = session.CustomActionData["installPath"];
            string vsixPath = Path.Combine(session.CustomActionData["sourcePath"], "PowerQuerySdk.vsix");

            if (!File.Exists(vsixPath))
            {
                MessageBox.Show(
                    string.Format(
                        "The following file is required:  {0}\n\nTo download, go to:  {1}"
                        , vsixPath
                        , "http://dakahn.gallery.vsassets.io/_apis/public/gallery/publisher/dakahn/extension/powerquerysdk/1.0.0.16/assetbyname/PowerQuerySdk.vsix"
                        ), "PowerQueryNet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ActionResult.Failure;
            }

            try
            {
                using (ZipArchive zipArchive = ZipFile.OpenRead(vsixPath))
                {
                    var dependencies = from entry in zipArchive.Entries
                                       where Path.GetDirectoryName(entry.FullName) == "Dependencies"
                                       where !String.IsNullOrEmpty(entry.Name)
                                       select entry;

                    foreach (ZipArchiveEntry zipArchiveEntry in dependencies)
                    {
                        zipArchiveEntry.ExtractToFile(Path.Combine(installPath, zipArchiveEntry.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting dependencies. " + ex.Message, "PowerQueryNet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveDependencies(Session session)
        {
            try
            {
                string installPath = session.CustomActionData["installPath"];

                DirectoryInfo di = new DirectoryInfo(installPath);
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
                Directory.Delete(installPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing dependencies. " + ex.Message, "PowerQueryNet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ActionResult.Success;
            }
            return ActionResult.Success;
        }
    }
}
