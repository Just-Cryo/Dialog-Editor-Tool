using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DialogEditor
{

    public partial class Form1 : Form
    {
        TreeContainer mainTree = new TreeContainer();
        dialogEntry en = new dialogEntry();
        bool lockUpdate = false;
        string curfile;
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string nodename = treeView1.SelectedNode.Text;
            if (treeView1.SelectedNode.Parent == null) //selected a trunk
            {

            }
            else
            {
                string parentname = treeView1.SelectedNode.Parent.Text;
                if (treeView1.SelectedNode.Parent.Parent == null) //selected a branch
                {

                }
                else //selected an entry
                {
                    string trunkname = treeView1.SelectedNode.Parent.Parent.Text;

                    en = mainTree.findEntry(trunkname, parentname, nodename);
                    entryTrunkBox.Text = en.tag.trunk;
                    entryBranchBox.Text = en.tag.branch;
                    entryIDBox.Text = en.tag.id.ToString();
                    groupBox2.Enabled = true;
                    en.display(ref treeView2);
                    updateDefaultTab();
                    updateScriptsTab();
                    updatePortraitsTab();
                    updateVoicelinesTab();
                    initConditinals();
                }
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (treeTextBox.Text.Length == 0)
            {
                TrunkContainer t;
                if (!mainTree.trunks.TryGetValue("default",out t))
                {
                    mainTree.trunks.Add("default", new TrunkContainer());
                    mainTree.parse_tree(ref treeView1);
                    treeTextBox.Clear();
                }
                else
                {
                    treeTextBox.BackColor = Color.FromArgb(255, 150, 150);
                }
            }
            else
            {
                TrunkContainer t;
                if (!mainTree.trunks.TryGetValue(treeTextBox.Text, out t))
                {
                    mainTree.trunks.Add(treeTextBox.Text, new TrunkContainer());
                    mainTree.parse_tree(ref treeView1);
                    treeTextBox.Clear();
                }
                else
                {
                    treeTextBox.BackColor = Color.FromArgb(255, 150, 150);
                }
            }
        }



        private void treeNewB_Click(object sender, EventArgs e)
        {
            string curtrunk = "";
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Parent != null)
                {
                    if (treeView1.SelectedNode.Parent.Parent != null)
                    {

                        curtrunk = treeView1.SelectedNode.Parent.Parent.Text;
                    }
                    else
                    {
                        curtrunk = treeView1.SelectedNode.Parent.Text;

                    }
                }
                else
                {
                    curtrunk = treeView1.SelectedNode.Text;
                }
            }
            else
            {
                curtrunk = "default";
                TrunkContainer t;
                if (!mainTree.trunks.TryGetValue("default", out t)) { 
                    mainTree.trunks.Add("default", new TrunkContainer());}
            }

                BranchContainer b;
                if (!mainTree.trunks[curtrunk].branches.TryGetValue(treeTextBox.Text, out b))
                {
                    mainTree.trunks[curtrunk].branches.Add(treeTextBox.Text, new BranchContainer());
                    mainTree.parse_tree(ref treeView1);
                    treeTextBox.Clear();
                }
                else
                {
                    treeTextBox.BackColor = Color.FromArgb(255, 150, 150);
                }
        }



        private void treeNewE_Click(object sender, EventArgs e)
        {
            string curtrunk = "";
            string curbranch = "";
            int curid;
            TreeNode tvn = treeView1.SelectedNode;
            if (treeTextBox.Text == "")
                treeTextBox.Text = "0";
            if (Int32.TryParse(treeTextBox.Text, out curid))
            {
                if (treeView1.SelectedNode != null) //something is selected
            {
                if (treeView1.SelectedNode.Parent != null) //either a branch or an entry is selected
                {
                    if (treeView1.SelectedNode.Parent.Parent != null)//an entry is selected
                    {
                        curbranch = treeView1.SelectedNode.Parent.Text;
                        if (curbranch == "[default branch]")
                            curbranch = "";
                        curtrunk = treeView1.SelectedNode.Parent.Parent.Text;
                            tvn = treeView1.SelectedNode.Parent;
                    }
                    else //a branch is selected
                    {

                        curbranch = treeView1.SelectedNode.Text;
                        if (curbranch == "[default branch]")
                            curbranch = "";
                        curtrunk = treeView1.SelectedNode.Parent.Text;
                    }

                }
                else //trunk is selected
                {
                    curtrunk = treeView1.SelectedNode.Text;
                    curbranch = "";
                    BranchContainer b;
                        if (!mainTree.trunks[curtrunk].branches.TryGetValue(curbranch, out b))
                        {
                            mainTree.trunks[curtrunk].branches.Add(curbranch, new BranchContainer());
                            if (curbranch == "")
                                tvn = tvn.Nodes.Add("[default branch]");
                            else
                                tvn = tvn.Nodes.Add(curbranch);
                            tvn.Name = curbranch;
                        }
                        else
                        {
                            treeTextBox.BackColor = Color.FromArgb(255, 150, 150);
                            return;
                        }
                }
              
            }
            else
            {
                curtrunk = "default";
                TrunkContainer t;
                    if (!mainTree.trunks.TryGetValue("default", out t))
                    {
                        mainTree.trunks.Add("default", new TrunkContainer());
                        tvn = treeView1.Nodes.Add("default");
                        tvn.Name = tvn.Text;
                    }
                    else
                    {
                        treeTextBox.BackColor = Color.FromArgb(255, 150, 150);
                        return;
                    }
                curbranch = "";
                BranchContainer b;
                    if (!mainTree.trunks[curtrunk].branches.TryGetValue(curbranch, out b))
                    {
                        mainTree.trunks[curtrunk].branches.Add(curbranch, new BranchContainer());
                        if (curbranch == "")
                            tvn = tvn.Nodes.Add("[default branch]");
                        else
                            tvn = tvn.Nodes.Add(curbranch);

                        tvn.Name = curbranch;
                    }
                    else
                    {
                        treeTextBox.BackColor = Color.FromArgb(255, 150, 150);
                        return;
                    }
            }


                dialogEntry entry;
                dialogEntry newentry = new dialogEntry(curtrunk, curid, curbranch,false);
                if (!mainTree.trunks[curtrunk].branches[curbranch].entries.TryGetValue(newentry.tag.name(), out entry))
                {
                    mainTree.trunks[curtrunk].branches[curbranch].entries.Add(newentry.tag.name(), newentry);
                    tvn = tvn.Nodes.Add(newentry.tag.name());

                    tvn.Name = tvn.Text;
                    //mainTree.parse_tree(ref treeView1);
                    treeTextBox.Clear();
                }
                else
                {
                    treeTextBox.BackColor = Color.FromArgb(255, 150, 150);

                }
            }
            else
            {
                treeTextBox.BackColor = Color.FromArgb(255, 150, 150);
                mainTree.parse_tree(ref treeView1);
                treeTextBox.Clear();

            }
        }


        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var sr = new StreamReader(openFileDialog1.FileName);
                mainTree.load_file(sr.ReadToEnd());
                mainTree.parse_tree(ref treeView1);
                curfile = openFileDialog1.FileName;





            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if (curfile != null)
            {

                var sr = new StreamWriter(curfile);
                mainTree.save_file(ref sr);
            }
           else
            {
                saveFile();
            }
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            TreeNode[] tv = treeView1.Nodes.Find(en.tag.trunk, false);
            if (tv.Length > 0)
            {
                tn = tv[0];
                tv = tn.Nodes.Find(en.tag.branch, false);
                if (tv.Length > 0)
                {
                    tn = tv[0];
                    int offset = 1;
                    while (tn.Nodes.ContainsKey(en.tag.trunk + (en.tag.id + offset).ToString() + en.tag.branch))
                    {
                        offset += 1;
                    }
                    tn = tn.Nodes.Add(en.tag.trunk + (en.tag.id + offset).ToString() + en.tag.branch);
                    tn.Name = tn.Text;
                    mainTree.trunks[en.tag.trunk].branches[en.tag.branch].entries.Add(
                        en.tag.trunk + (en.tag.id + offset).ToString() + en.tag.branch, 
                        new dialogEntry(en.tag.trunk ,(en.tag.id + offset),en.tag.branch,false));
                    treeView1.SelectedNode = tn;
                }
            }
        }






        private void treeTextBox_TextChanged(object sender, EventArgs e)
        {
            treeTextBox.BackColor = Color.FromArgb(255, 255, 255);
        }



        public void updateEntryTree()
        {
            en.display(ref treeView2);
        }


        //---------------------------
        //-------------------------------
        //----DEFAULT TAB--------------------
        //---------------------------------------
        //-------------------------------------------

        public void updateDefaultTab()
        {
            d_t_cbox.Text = en.defaultText.character;
            d_t_nbox.Text = en.defaultText.nameid;
            d_t_dbox.Text = en.defaultText.text;
            d_t_sbox.Text = en.defaultOptions.speed.ToString();
            d_t_exitcheck.Checked = en.conditionals[0].defaultExit;
            d_t_nexttrunk.Text = en.conditionals[0].defaultTag.trunk;
            d_t_nextid.Text = en.conditionals[0].defaultTag.id.ToString();
            d_t_nextbranch.Text = en.conditionals[0].defaultTag.branch;
        }

        private void d_t_dbox_TextChanged(object sender, EventArgs e) //dialog box
        {
            en.defaultText.text = d_t_dbox.Text;
            updateEntryTree();
            
        }

        private void d_t_cbox_TextChanged(object sender, EventArgs e) //character box
        {
            en.defaultText.character = d_t_cbox.Text;
            updateEntryTree();

        }

        private void d_t_nbox_TextChanged(object sender, EventArgs e) //name id box
        {
            en.defaultText.nameid = d_t_nbox.Text;
            updateEntryTree();

        }

        private void d_t_sbox_TextChanged(object sender, EventArgs e) //speed box
        {
            d_t_sbox.BackColor = Color.FromArgb(255, 255, 255);
            float t = 0;
            if (float.TryParse(d_t_sbox.Text, out t))
                {
                en.defaultOptions.speed = t;
                updateEntryTree();
            }
            else
                d_t_sbox.BackColor = Color.FromArgb(255, 150, 150);

        }



        private void d_t_nexttrunk_TextChanged(object sender, EventArgs e)
        {

            en.conditionals[0].defaultTag.trunk = d_t_nexttrunk.Text;
            if (c_t_cselect.SelectedIndex == 0)
                c_t_dtrunk.Text = d_t_nexttrunk.Text;
            updateEntryTree();
        }

        private void d_t_nextid_TextChanged(object sender, EventArgs e)
        {
            int tv = 0;
            if (Int32.TryParse(d_t_nextid.Text,out tv))
            {

                en.conditionals[0].defaultTag.id = tv;
                if (c_t_cselect.SelectedIndex == 0)
                    c_t_did.Text = d_t_nextid.Text;
                updateEntryTree();
            }
        }

        private void d_t_nextbranch_TextChanged(object sender, EventArgs e)
        {

            en.conditionals[0].defaultTag.branch = d_t_nextbranch.Text;
            if (c_t_cselect.SelectedIndex == 0)
                c_t_dbranch.Text = d_t_nextbranch.Text;
            updateEntryTree();
        }

        private void d_t_exitcheck_CheckedChanged(object sender, EventArgs e)
        {

            en.conditionals[0].defaultExit = d_t_exitcheck.Checked;
            if (c_t_cselect.SelectedIndex == 0)
                c_t_dquit.Checked = d_t_exitcheck.Checked;
            updateEntryTree();
        }





        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////

        //---------------------------
        //-------------------------------
        //----SCRIPTS TAB--------------------
        //---------------------------------------
        //-------------------------------------------

        public void updateScriptsTab()
        {
            s_t_scriptSelect.Items.Clear();
            if (en.scripts.Count > 0)
            {
                foreach (scriptObject s in en.scripts)
            {
                s_t_scriptSelect.Items.Add(s.domain+"."+s.script);
            }

                s_t_scriptSelect.SelectedIndex = 0;
            }
        }

        private void s_t_scriptSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indx = s_t_scriptSelect.SelectedIndex;
            int selindx = s_t_dselect.FindStringExact(en.scripts[indx].domain);
            if (selindx == -1)
                selindx = s_t_dselect.Items.Count - 1;
            s_t_dselect.SelectedIndex = selindx;

            s_t_sbox.Text = en.scripts[indx].script;
            string temp_t = "";
            foreach (string val in en.scripts[indx].values)
            {
                temp_t += val+",";
            }
            s_t_argbox.Text = temp_t.Remove(temp_t.Length-1,1);
        }

        private void s_t_new_Click(object sender, EventArgs e)
        {
            en.scripts.Add(new scriptObject());
            updateEntryTree();
            updateScriptsTab();
            s_t_scriptSelect.SelectedIndex  = s_t_scriptSelect.Items.Count-1;
        }

        private void s_t_del_Click(object sender, EventArgs e)
        {
            int indx = s_t_scriptSelect.SelectedIndex;
            en.scripts.RemoveAt(indx);
            updateEntryTree();
            updateScriptsTab();
        }

        private void s_t_dselect_SelectedIndexChanged(object sender, EventArgs e)
        {
            s_t_dselect.BackColor = Color.FromArgb(255, 255, 255);
            if (en.scripts.Count > 0)
            {
                int indx = s_t_scriptSelect.SelectedIndex;
                if (s_t_dselect.Text != "UNKNOWN")
                {
                    en.scripts[indx].domain = s_t_dselect.Text;
                    s_t_scriptSelect.Items[indx] = (en.scripts[indx].domain + "." + en.scripts[indx].script);
                }
                else
                {
                    s_t_dselect.BackColor = Color.FromArgb(255, 150, 150);
                }
            }
            updateEntryTree();
        }

        private void s_t_sbox_TextChanged(object sender, EventArgs e)
        {
            int indx = s_t_scriptSelect.SelectedIndex;
            en.scripts[indx].script = s_t_sbox.Text;
            s_t_scriptSelect.Items[indx] = (en.scripts[indx].domain + "." + en.scripts[indx].script);
            updateEntryTree();
        }
        private void s_t_argbox_TextChanged(object sender, EventArgs e)
        {
            int indx = s_t_scriptSelect.SelectedIndex;
            string[] args = s_t_argbox.Text.Split(',');
            for (int i =0; i < args.Length;++i)
            {
                    if (en.scripts[indx].values.Count>=i)
                        en.scripts[indx].values[i] = args[i];
                    else
                        en.scripts[indx].values.Add(args[i]);
            }
            updateEntryTree();

        }


        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////


        //---------------------------
        //-------------------------------
        //----CONDITIONALS TAB---------------
        //---------------------------------------
        //-------------------------------------------

        public void initConditinals()
        {

            c_t_cselect.Items.Clear();
            if (en.conditionals.Count > 0)
            {
                foreach (conditionalObject o in en.conditionals)
                {
                    c_t_cselect.Items.Add(o.conditional.Domain + "." + o.conditional.Variable + " " + o.conditional.comparator + " " + o.conditional.value);
                }

                c_t_cselect.SelectedIndex = 0;
            }

        }
        public void updateConditionalsTab()
        {




        }




        private void c_t_cselect_SelectedIndexChanged(object sender, EventArgs e)
        {

            int indx = c_t_cselect.SelectedIndex;
            int selindx = c_t_dsel.FindStringExact(en.conditionals[indx].conditional.Domain);
            if (selindx == -1)
                selindx = c_t_dsel.Items.Count - 1;
            c_t_dsel.SelectedIndex = selindx;

            c_t_cvar.Text = en.conditionals[indx].conditional.Variable;

            selindx = c_t_comparator.FindStringExact(en.conditionals[indx].conditional.comparator);
            if (selindx == -1)
                selindx = 0;
            c_t_comparator.SelectedIndex = selindx;

            c_t_cval.Text = en.conditionals[indx].conditional.value.ToString();

            c_t_isjump.Checked = en.conditionals[indx].jump;


            c_t_jtrunk.Text = en.conditionals[indx].jumpTarget.trunk;
            c_t_jid.Text = en.conditionals[indx].jumpTarget.id.ToString();
            c_t_jbranch.Text= en.conditionals[indx].jumpTarget.branch;

            c_t_dquit.Checked = en.conditionals[indx].defaultExit;
            c_t_dtrunk.Text = en.conditionals[indx].defaultTag.trunk;
            c_t_did.Text = en.conditionals[indx].defaultTag.id.ToString();
            c_t_dbranch.Text = en.conditionals[indx].defaultTag.branch;


            if (!en.conditionals[indx].jump)
            {

                c_t_oselect.Items.Clear();
                if (en.conditionals[indx].options !=null)
                {
                    foreach (optionObject o in en.conditionals[indx].options)
                    {
                        c_t_oselect.Items.Add(o.tag.name());
                    }
                    if (c_t_oselect.Items.Count>0)
                        c_t_oselect.SelectedIndex = 0;
                }

            }


        }

        private void c_t_cnew_Click(object sender, EventArgs e)
        {
            en.conditionals.Add(new conditionalObject());
            int indx = en.conditionals.Count-1;
            conditionalObject o = en.conditionals[indx];
            c_t_cselect.Items.Add(o.conditional.Domain + "." + o.conditional.Variable + " " + o.conditional.comparator + " " + o.conditional.value);

            updateEntryTree();
            //updateConditionalsTab();
            c_t_cselect.SelectedIndex = c_t_cselect.Items.Count - 1;
        }

        private void c_t_cdel_Click(object sender, EventArgs e)
        {

            int indx = c_t_cselect.SelectedIndex;
            en.conditionals.RemoveAt(indx);
            c_t_cselect.Items.RemoveAt(indx);
            updateEntryTree();
            //updateConditionalsTab();
        }

        private void c_t_dsel_SelectedIndexChanged(object sender, EventArgs e)
        {
            c_t_dsel.BackColor = Color.FromArgb(255, 255, 255);
            if (en.conditionals.Count > 0)
            {
                int indx = c_t_cselect.SelectedIndex;
                if (c_t_dsel.Text != "UNKNOWN")
                {
                    en.conditionals[indx].conditional.Domain = c_t_dsel.Text;
                    c_t_cselect.Items[indx] = (
                        en.conditionals[indx].conditional.Domain + "." + en.conditionals[indx].conditional.Variable + " " + 
                        en.conditionals[indx].conditional.comparator + " " + en.conditionals[indx].conditional.value
                        );
                } 
                else
                {
                    c_t_dsel.BackColor = Color.FromArgb(255, 150, 150);
                }
            }
            updateEntryTree();
        }

        private void c_t_cvar_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            en.conditionals[indx].conditional.Variable = c_t_cvar.Text;

            c_t_cselect.Items[indx] = (
                          en.conditionals[indx].conditional.Domain + "." + en.conditionals[indx].conditional.Variable + " " +
                          en.conditionals[indx].conditional.comparator + " " + en.conditionals[indx].conditional.value
                          );
            updateEntryTree();
        }

        private void c_t_comparator_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            en.conditionals[indx].conditional.comparator = c_t_comparator.Text;
            c_t_cselect.Items[indx] = (
                        en.conditionals[indx].conditional.Domain + "." + en.conditionals[indx].conditional.Variable + " " + 
                        en.conditionals[indx].conditional.comparator + " " + en.conditionals[indx].conditional.value
                        );
                
        }

        private void c_t_cval_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            float tf;
            if (float.TryParse(c_t_cval.Text, out tf))
            {
                en.conditionals[indx].conditional.value = tf;
                c_t_cval.BackColor = Color.FromArgb(255, 255, 255);
                c_t_cselect.Items[indx] = (
                              en.conditionals[indx].conditional.Domain + "." + en.conditionals[indx].conditional.Variable + " " +
                              en.conditionals[indx].conditional.comparator + " " + en.conditionals[indx].conditional.value
                              );
            }
            else
            {
                c_t_cval.BackColor = Color.FromArgb(255, 150, 150);
            }
            updateEntryTree();
        }

        private void c_t_isjump_CheckedChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            en.conditionals[indx].jump = c_t_isjump.Checked;
            c_t_optionparent.Enabled = !en.conditionals[indx].jump;

            updateEntryTree();

        }

        private void c_t_jtrunk_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            en.conditionals[indx].jumpTarget.trunk = c_t_jtrunk.Text;
            updateEntryTree();
        }

        private void c_t_jid_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            int ti = 0;
            if (Int32.TryParse(c_t_jid.Text,out ti))
            {
                en.conditionals[indx].jumpTarget.id = ti;
                updateEntryTree();
                c_t_jid.BackColor = Color.FromArgb(255, 255, 255);

            }
            else
            {
                c_t_jid.BackColor = Color.FromArgb(255, 150, 150);
            }

        }

        private void c_t_jbranch_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            en.conditionals[indx].jumpTarget.branch = c_t_jbranch.Text;
            updateEntryTree();

        }

        private void c_t_oselect_SelectedIndexChanged(object sender, EventArgs e)
        {


            int indx = c_t_cselect.SelectedIndex;
            int indx2 = c_t_oselect.SelectedIndex;
            if (indx2 >= 0 && en.conditionals[indx].options.Count>0)
            {
                c_t_oquit.Checked = en.conditionals[indx].options[indx2].exitDialog;

                c_t_odiag.Text = en.conditionals[indx].options[indx2].optionDialog;

                c_t_otrunk.Text = en.conditionals[indx].options[indx2].tag.trunk;
                c_t_oid.Text = en.conditionals[indx].options[indx2].tag.id.ToString();
                c_t_obranch.Text = en.conditionals[indx].options[indx2].tag.branch;
                c_t_oquit.Enabled = true;
                c_t_odiag.Enabled = true;
                c_t_otrunk.Enabled = true;
                c_t_oid.Enabled = true;
                c_t_obranch.Enabled = true;
            }
            else
            {
                c_t_oquit.Checked = false;
                c_t_odiag.Text = "";
                c_t_otrunk.Text = "";
                c_t_oid.Text = "";
                c_t_obranch.Text = "";
                c_t_oquit.Enabled = false;
                c_t_odiag.Enabled = false;
                c_t_otrunk.Enabled = false;
                c_t_oid.Enabled = false;
                c_t_obranch.Enabled = false;

            }
        }

        private void c_t_onew_Click(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            en.conditionals[indx].options.Add(new optionObject());
            int indx2 = en.conditionals[indx].options.Count-1;
            optionObject o = en.conditionals[indx].options[indx2];
            c_t_oselect.Items.Add(o.tag.name());
            updateEntryTree();
            //updateConditionalsTab();
            c_t_oselect.SelectedIndex = c_t_oselect.Items.Count - 1;
        }

        private void c_t_odel_Click(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            int indx2 = c_t_oselect.SelectedIndex;
            if (indx >= 0 && indx2 >= 0)
            {
                en.conditionals[indx].options.RemoveAt(indx2);
                c_t_oselect.Items.RemoveAt(indx2);
                updateEntryTree();
            }
            //updateConditionalsTab();
        }

        private void c_t_oquit_CheckedChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            int indx2 = c_t_oselect.SelectedIndex;
            if (indx2 != -1 && indx >= 0)
            {
                en.conditionals[indx].options[indx2].exitDialog = c_t_oquit.Checked;
                updateEntryTree();
            }
        }

        private void c_t_otrunk_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            int indx2 = c_t_oselect.SelectedIndex;
            if (indx2 != -1&& indx >= 0)
            {
                en.conditionals[indx].options[indx2].tag.trunk = c_t_otrunk.Text;
                c_t_oselect.Items[indx2] = en.conditionals[indx].options[indx2].tag.name();
                updateEntryTree();
            }
        }

        private void c_t_oid_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            int indx2 = c_t_oselect.SelectedIndex;
            if (indx2 != -1&& indx >= 0)
            {
                int ti = 0;
                if (Int32.TryParse(c_t_oid.Text, out ti))
                {
                    en.conditionals[indx].options[indx2].tag.id = ti;
                    c_t_oselect.Items[indx2] = en.conditionals[indx].options[indx2].tag.name();
                    updateEntryTree();
                    c_t_did.BackColor = Color.FromArgb(255, 255, 255);

                }
                else
                {
                    c_t_did.BackColor = Color.FromArgb(255, 150, 150);
                }
            }
        }

        private void c_t_obranch_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            int indx2 = c_t_oselect.SelectedIndex;
            if (indx2 != -1&& indx >= 0)
            {
                en.conditionals[indx].options[indx2].tag.branch = c_t_obranch.Text;
                c_t_oselect.Items[indx2] = en.conditionals[indx].options[indx2].tag.name();
                updateEntryTree();
            }
        }

        private void c_t_odiag_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            int indx2 = c_t_oselect.SelectedIndex;
            if (indx2 != -1&& indx >=0)
            {
                en.conditionals[indx].options[indx2].optionDialog = c_t_odiag.Text;
                updateEntryTree();
            }
        }

        private void c_t_dquit_CheckedChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex; 
            if (indx >=0)
            {
                en.conditionals[indx].defaultExit = c_t_dquit.Checked;
                if (c_t_cselect.SelectedIndex == 0)
                    updateDefaultTab();
                updateEntryTree();
            }
        }

        private void c_t_dtrunk_TextChanged(object sender, EventArgs e)
        {

            int indx = c_t_cselect.SelectedIndex;
            if (indx >= 0)
            {
                en.conditionals[indx].defaultTag.trunk = c_t_dtrunk.Text;
                if (c_t_cselect.SelectedIndex == 0)
                    updateDefaultTab();
                updateEntryTree();
            }
            else
            {
                c_t_dtrunk.Text = "";
            }
        }

        private void c_t_did_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            if (indx >= 0)
            {
                int ti = 0;
                if (Int32.TryParse(c_t_did.Text, out ti))
                {
                    en.conditionals[indx].defaultTag.id = ti;
                    updateEntryTree();
                    if (c_t_cselect.SelectedIndex == 0)
                        updateDefaultTab();
                    c_t_did.BackColor = Color.FromArgb(255, 255, 255);

                }
                else
                {
                    c_t_did.BackColor = Color.FromArgb(255, 150, 150);
                }
            }
            else
            {
                c_t_did.Text = "";

            }
        }

        private void c_t_dbranch_TextChanged(object sender, EventArgs e)
        {
            int indx = c_t_cselect.SelectedIndex;
            if (indx >= 0)
            {
                en.conditionals[indx].defaultTag.branch = c_t_dbranch.Text;
                if (c_t_cselect.SelectedIndex == 0)
                    updateDefaultTab();
                updateEntryTree();
            }
            else
            {
                c_t_dbranch.Text = "";
            }
        }



        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////






        //---------------------------
        //-------------------------------
        //----PORTRAITS TAB------------------
        //---------------------------------------
        //-------------------------------------------

        public void updatePortraitsTab()
        {
            p_t_select.Items.Clear();
            if (en.portraits.Count > 0)
            {
                foreach (portraitObject p in en.portraits)
                {
                    p_t_select.Items.Add(p.character + "." + p.pose);
                }

                p_t_select.SelectedIndex = 0;
            }
        }


        private void p_t_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indx = p_t_select.SelectedIndex;
            p_t_pcbox.Text = en.portraits[indx].character;
            p_t_ppbox.Text = en.portraits[indx].pose;
            int selindx = 0;
            switch (en.portraits[indx].side)
            {
                case "L":
                    selindx = 0;
                        break;
                    
                case "R":
                    selindx = 1;
                     break; 
                case "C":
                    selindx = 2;
                    break;
                default:
                    selindx = 2;
                    break;
            }
            p_t_asel.SelectedIndex = selindx;
            p_t_obox.Text = en.portraits[indx].offset.ToString();
        }

        private void p_t_pnew_Click(object sender, EventArgs e)
        {

            int indx = p_t_select.SelectedIndex;
            en.portraits.Add(new portraitObject());
            p_t_select.Items.Add(en.portraits[indx].character + "." + en.portraits[indx].pose);
            updateEntryTree();
            updatePortraitsTab();
            p_t_select.SelectedIndex = p_t_select.Items.Count-1;
        }

        private void p_t_pdel_Click(object sender, EventArgs e)
        {
            int indx = p_t_select.SelectedIndex;
            en.portraits.RemoveAt(indx);
            p_t_select.Items.RemoveAt(indx);
            updateEntryTree();
            updatePortraitsTab();
        }

        private void p_t_pcbox_TextChanged(object sender, EventArgs e)
        {

            int indx = p_t_select.SelectedIndex;
            en.portraits[indx].character = p_t_pcbox.Text;
            p_t_select.Items[indx] = (en.portraits[indx].character + "." + en.portraits[indx].pose);
            updateEntryTree();
        }

        private void p_t_ppbox_TextChanged(object sender, EventArgs e)
        {
            int indx = p_t_select.SelectedIndex;
            en.portraits[indx].pose = p_t_ppbox.Text;
            p_t_select.Items[indx] = (en.portraits[indx].character + "." + en.portraits[indx].pose);
            updateEntryTree();

        }

        private void p_t_asel_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indx = p_t_select.SelectedIndex;
            en.portraits[indx].side = p_t_asel.Text;
            updateEntryTree();
            
        }

        private void p_t_obox_TextChanged(object sender, EventArgs e)
        {
            int indx = p_t_select.SelectedIndex;
            float tf = 0;
            if (float.TryParse(p_t_obox.Text,out tf))
            {
                en.portraits[indx].offset = tf;
                p_t_obox.BackColor = Color.FromArgb(255, 255, 255);
                updateEntryTree();

            }
            else
            {
                p_t_obox.BackColor = Color.FromArgb(255, 150, 150);
            }




        }



        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////






        //---------------------------
        //-------------------------------
        //----VOICELINES TAB-----------------
        //---------------------------------------
        //-------------------------------------------

        public void updateVoicelinesTab()
        {
            v_t_select.Items.Clear();
            if (en.conditionals.Count > 0)
            {
                foreach (voicelineObject v in en.voicelines)
                {
                    v_t_select.Items.Add(v.character + "." + v.voiceline);
                }
            }
        }

        private void v_t_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indx = v_t_select.SelectedIndex;
            v_t_vchar.Text = en.voicelines[indx].character;
            v_t_vvl.Text = en.voicelines[indx].voiceline;
            v_t_vvol.Text = en.voicelines[indx].volume.ToString();
        }

        private void v_t_vnew_Click(object sender, EventArgs e)
        {

            int indx = v_t_select.SelectedIndex;
            en.voicelines.Add(new voicelineObject());
            v_t_select.Items.Add(en.voicelines[indx].character + "." + en.voicelines[indx].voiceline);
            updateEntryTree();
            updateVoicelinesTab();
            v_t_select.SelectedIndex = v_t_select.Items.Count-1;
        }

        private void v_t_vdel_Click(object sender, EventArgs e)
        {
            int indx = v_t_select.SelectedIndex;
            en.voicelines.RemoveAt(indx);
            v_t_select.Items.RemoveAt(indx);
            updateEntryTree();
            updateVoicelinesTab();
        }

        private void v_t_vchar_TextChanged(object sender, EventArgs e)
        {

            int indx = v_t_select.SelectedIndex;
            en.voicelines[indx].character = v_t_vchar.Text;
            v_t_select.Items[indx] = (en.voicelines[indx].character + "." + en.voicelines[indx].voiceline);
            updateEntryTree();
        }

        private void v_t_vvl_TextChanged(object sender, EventArgs e)
        {

            int indx = v_t_select.SelectedIndex;
            en.voicelines[indx].voiceline = v_t_vvl.Text;
            v_t_select.Items[indx] = (en.voicelines[indx].character + "." + en.voicelines[indx].voiceline);
            updateEntryTree();
        }

        private void v_t_vvol_TextChanged(object sender, EventArgs e)
        {
            int indx = v_t_select.SelectedIndex;
            float tf = 0;
            if (float.TryParse(v_t_vvol.Text, out tf))
            {
                en.voicelines[indx].volume = tf;
                v_t_vvol.BackColor = Color.FromArgb(255, 255, 255);
                updateEntryTree();

            }
            else
            {
                v_t_vvol.BackColor = Color.FromArgb(255, 150, 150);
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
        private void saveFile()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var sr = new StreamWriter(saveFileDialog1.FileName);
                mainTree.save_file(ref sr);
                curfile = saveFileDialog1.FileName;
            }
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
        }




        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////
        ///
        private void entryMoveE_Click(object sender, EventArgs e)
        {
            string newt = entryTrunkBox.Text;
            string newb = entryBranchBox.Text;
            string newid = entryIDBox.Text;
            TreeNode tn = treeView1.SelectedNode;
            if (newt != en.tag.trunk || newb != en.tag.branch || newid != en.tag.id.ToString())
            {
                 if (!mainTree.trunks.ContainsKey(newt))
                {
                    mainTree.trunks.Add(newt,new TrunkContainer());
                    tn = treeView1.Nodes.Add(newt);
                    tn.Name = tn.Text;
                }
                 else
                {
                    TreeNode[] tv = treeView1.Nodes.Find(newt, false);
                    if (tv.Length > 0)
                        tn = tv[0];
                }
                if (!mainTree.trunks[newt].branches.ContainsKey(newb))
                {
                    mainTree.trunks[newt].branches.Add(newb, new BranchContainer());
                    tn = tn.Nodes.Add(newb != "" ? newb : "[default branch]");
                    tn.Name = newb;
                }
                else
                {
                    TreeNode[] tv = tn.Nodes.Find(newb,false);
                    if (tv.Length > 0)
                        tn = tv[0];
                }
                if (!mainTree.trunks[newt].branches[newb].entries.ContainsKey(newt + newid + newb))
                {
                    tn = tn.Nodes.Add(newt + newid + newb);
                    tn.Name = tn.Text;

                    TreeNode[] tv = treeView1.Nodes.Find(en.tag.trunk, false);
                    if (tv.Length > 0)
                    {
                        tn = tv[0];
                        tv = tn.Nodes.Find(en.tag.branch,false);
                        if (tv.Length > 0)
                        {
                            tn = tv[0];
                            //tn.Nodes.RemoveByKey(en.tag.name());
                            mainTree.trunks[en.tag.trunk].branches[en.tag.branch].entries.Remove(en.tag.name());
                            en.tag.trunk = newt;
                            en.tag.branch = newb;
                            en.tag.id = Int32.Parse(newid);
                            mainTree.trunks[newt].branches[newb].entries.Add(newt + newid + newb, en);
                            mainTree.parse_tree(ref treeView1);
                            groupBox2.Enabled = false;
                        }
                    }


                }
                {
                    TreeNode[] tv = tn.Nodes.Find(newt + newid + newb,false);
                    if (tv.Length > 0)
                        tn = tv[0];
                }
            }
        }

        private void entryDelE_Click(object sender, EventArgs e)
        {

            mainTree.trunks[en.tag.trunk].branches[en.tag.branch].entries.Remove(en.tag.name());
            en = null;
            mainTree.parse_tree(ref treeView1);
            groupBox2.Enabled = false;
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {

        }


    }
}
