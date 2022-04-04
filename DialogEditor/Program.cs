using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Windows.Forms;

namespace DialogEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    public class dialogTag
    {
        public string trunk;
        public string branch;
        public int id;
        public string name()
        {
            return (trunk + id.ToString() + branch);
        }
        public dialogTag(string _t = "default", int _i = 0,string _b = "")
        {
            trunk = _t;
            id = _i;
            branch = _b;
        }
    }
    public class scriptObject
    {
        public string domain;
        public string script;
        public List<string> values = new List<string>();
        public scriptObject(string _d = "Global",string _s = "")
        {
            domain = _d;
            script = _s;
            values = new List<string>();
        }
    }
    public class portraitObject
    {
        public string character;
        public string pose;
        public string side;
        public float offset;
        public portraitObject(string _c ="", string _p = "", string _s = "", float _o = 0)
        {
            character = _c;
            pose = _p;
            side = _s;
            offset = _o;
        }
    }
    public class voicelineObject
    {
        public string character;
        public string voiceline;
        public float volume;
        public voicelineObject(string _c = "", string _vl = "", float _v = 100)
        {
            character = _c;
            voiceline = _vl;
            volume = _v;
        }
    }
    public class conditionalCheck
    {
        public string Domain;
        public string Variable;
        public float value;
        public string comparator;
    }
    public class optionObject
    {
        public string optionDialog;
        public bool exitDialog;
        public dialogTag tag;
        public optionObject(string _d ="default", dialogTag _t = null, bool _e = false)
        {
            optionDialog = _d;
            if (_t == null)
                tag = new dialogTag();
            else
                tag = _t;
            exitDialog = _e;
        }
    }
    public class conditionalObject
    {
        public conditionalCheck conditional = new conditionalCheck();
        public List<optionObject> options = new List<optionObject>();
        public bool defaultExit;
        public dialogTag defaultTag = new dialogTag();
        public bool jump;
        public dialogTag jumpTarget = new dialogTag();
    }
    public class defaultScript
    {
        public string text;
        public string character;
        public string nameid;
    }
    public class dialogOptions
    {
        public float speed;
    }
    public class dialogEntry : System.Object
    {
        public dialogTag tag = new dialogTag();
        public List<scriptObject> scripts = new List<scriptObject>();
        public List<portraitObject> portraits = new List<portraitObject>();
        public List<voicelineObject> voicelines = new List<voicelineObject>();
        public defaultScript defaultText = new defaultScript();
        public List<conditionalObject> conditionals = new List<conditionalObject>();
        public dialogOptions defaultOptions = new dialogOptions();
        public dialogEntry(string _t = "default", int _i = 0, string _b = "",bool empty = true)
        {
            this.tag.trunk = _t;
            this.tag.branch = _b;
            this.tag.id = _i;
            defaultOptions.speed = 100;
            if (!empty)
            {
                conditionals.Add(new conditionalObject());
                conditionals[0].defaultTag.branch = this.tag.branch;
                conditionals[0].defaultTag.trunk = this.tag.trunk;
                conditionals[0].defaultTag.id = this.tag.id + 1;
                conditionals[0].conditional.Domain = "Global";
                conditionals[0].conditional.Variable = "True";
                conditionals[0].conditional.comparator = "=";
                conditionals[0].conditional.value = 1;
            }
        }
        public bool display(ref TreeView tree)
        {
            tree.Nodes.Clear();
           // tree.Nodes.Add(tag.name());
           // tree.Name = tag.name();
            tree.Parent.Text = tag.name();
            TreeNode n = tree.Nodes.Add("Scripts");
            foreach (scriptObject s in scripts)
            {
                n.Nodes.Add(s.domain + ":" + s.script);
            }
            n = tree.Nodes.Add("Portraits");
            foreach (portraitObject p in portraits)
            {
                n.Nodes.Add(p.character + ":" + p.pose);
            }
            n = tree.Nodes.Add("Voicelines");
            foreach(voicelineObject v in voicelines)
            {
                n.Nodes.Add(v.character + ":" + v.voiceline);
            }
            n = tree.Nodes.Add("[Main]");
            n.Nodes.Add(defaultText.text);
            n = tree.Nodes.Add("rules");
            foreach(conditionalObject c in conditionals)
            {
                if (!c.jump)
                {
                    TreeNode n2 = n.Nodes.Add(c.conditional.Variable + ":" + c.conditional.comparator + ":" + c.conditional.value);
                    if (c.options != null)
                    {
                        foreach (optionObject o in c.options)
                        {
                            n2.Nodes.Add(o.optionDialog + "->" + o.tag.name());
                        }
                    }
                    n.Nodes.Add(c.defaultTag.name());
                }
                else
                {
                    n.Nodes.Add("Jump:" + c.jumpTarget.name());
                }
            }
            tree.ExpandAll();
            return true;
        }
    }

    public class BranchContainer
    {
        public Dictionary<string,dialogEntry> entries = new Dictionary<string, dialogEntry>();

    }
    public class TrunkContainer
    {
        public Dictionary<string, BranchContainer> branches = new Dictionary<string, BranchContainer>();
    }
    public class TreeContainer
    {
        public Dictionary<string, TrunkContainer> trunks = new Dictionary<string, TrunkContainer>();
        public bool parse_tree(ref TreeView tree)
        {
            tree.Nodes.Clear();
            foreach(KeyValuePair<string, TrunkContainer> t in trunks)
            {
                TreeNode tn = tree.Nodes.Add(t.Key);

                tn.Name = tn.Text;
                foreach (KeyValuePair <string, BranchContainer> b in t.Value.branches)
                {
                    TreeNode bn;
                    if (b.Key == "")
                        bn = tn.Nodes.Add("[default branch]");
                    else
                        bn = tn.Nodes.Add(b.Key);

                    bn.Name = b.Key;
                    foreach (KeyValuePair < string, dialogEntry> e in b.Value.entries)
                    {
                        TreeNode ent = bn.Nodes.Add(e.Value.tag.name());
                        ent.Name = ent.Text;
                    }
                }
            }
            return true;
        }

        public bool load_file(string _data)
        {
            string[] lines = _data.Split(System.Environment.NewLine.ToCharArray());

            trunks.Clear();
            dialogTag curtag = new dialogTag();
            bool defaultConditional = true;
            conditionalObject cO = new conditionalObject();
            foreach (string l in lines) {
                if (l.Length == 0)
                    continue;
                char code = l[0];
                string nl = l.Remove(0, 1);
                string[] arg = nl.Split(':');
                switch (code)
                {
                    case '!': //new entry
                        {
                            TrunkContainer tc;
                            BranchContainer bc;
                            curtag.trunk = arg[0];
                            curtag.branch = arg[2];
                            curtag.id = Int32.Parse(arg[1]);
                            if (!trunks.TryGetValue(curtag.trunk, out tc))
                                trunks.Add(curtag.trunk, new TrunkContainer());
                            if (!trunks[curtag.trunk].branches.TryGetValue(curtag.branch, out bc))
                                trunks[curtag.trunk].branches.Add(curtag.branch, new BranchContainer());
                            trunks[curtag.trunk].branches[curtag.branch].entries.Add(curtag.name(), new dialogEntry(curtag.trunk, curtag.id, curtag.branch));
                            break;
                        }
                    case '$': //portrait 
                        {
                            trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].portraits.Add(
                                new portraitObject(arg[0], arg[1], arg[2],Int32.Parse(arg[3])));
                            break;
                        }
                    case '#': //voiceline
                        {
                            trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].voicelines.Add(
                                new voicelineObject(arg[0], arg[1], Int32.Parse(arg[2])));
                            break;
                        }
                    case '%': //script
                        {
                            trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].scripts.Add(new scriptObject(arg[0], arg[1]));
                            scriptObject sO = trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].scripts.Last();
                            for(int i =2; i < arg.Length; ++i)
                            {
                                sO.values.Add(arg[i]);
                            }
                            break;
                        }
                    case '~': //default text
                        {
                            trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].defaultText.character = arg[0];
                            trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].defaultText.nameid = arg[1];
                            trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].defaultText.text = arg[2];
                            break;
                        }
                    case '*': //default setting
                        {
                            switch (arg[0])
                            {
                                case "Speed":
                                    {
                                        trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].defaultOptions.speed = Int32.Parse(arg[1]);
                                        break;
                                    }
                            }
                            break;
                        }
                    case '@': //conditional
                        {
                            trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Add(new conditionalObject());
                            cO = trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Last();
                            cO.conditional.Domain = arg[0];
                            cO.conditional.Variable = arg[1];
                            cO.conditional.comparator = arg[2];
                            cO.conditional.value = float.Parse(arg[3]);
                            cO.options = new List<optionObject>();
                            defaultConditional = false;
                            break;
                        }
                    case '&': //selectable option
                        {
                            bool exit = false;

                            if (arg[0] != "")
                            {
                                if (arg[0].ToArray()[0] == '*')//exit
                                {
                                    nl = nl.Remove(0, 1);
                                    arg = nl.Split(':');
                                    exit = true;
                                }
                            }
                            if (arg[0] == "")
                            {
                                arg[0] = curtag.trunk;
                            }
                            if (arg[1] == "")
                            {
                                arg[1] = (curtag.id + 1).ToString();
                            }
                            if (arg[2] == "")
                            {
                                arg[2] = curtag.branch;
                            }
                            if (defaultConditional)
                            {
                                trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Add(new conditionalObject());
                                cO = trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Last();
                                cO.conditional = new conditionalCheck();
                                cO.conditional.Domain = "Global";
                                cO.conditional.Variable = "True";
                                cO.conditional.comparator = "=";
                                cO.conditional.value = 1;
                                cO.options = new List<optionObject>();
                                defaultConditional = false;
                               // trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Add(cO);
                            }
                            cO.options.Add(new optionObject(arg[3], new dialogTag(arg[0], Int32.Parse(arg[1]), arg[2]), exit));
                            break;
                        }
                    case '>': //dialog jump
                        {
                            bool exit = false;
                            if (arg[0].ToArray()[0] == '*')//exit
                            {
                                nl = nl.Remove(0, 1);
                                arg = nl.Split(':');
                                exit = true;
                            }
                            if (arg[0] == "")
                            {
                                arg[0] = curtag.trunk;
                            }
                            if (arg[1] == "")
                            {
                                arg[1] = (curtag.id + 1).ToString();
                            }
                            if (arg[2] == "")
                            {
                                arg[2] = curtag.branch;
                            }
                            if (defaultConditional)
                            {
                                trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Add(new conditionalObject());
                                cO = trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Last();
                                cO.conditional = new conditionalCheck();
                                cO.conditional.Domain = "Global";
                                cO.conditional.Variable = "True";
                                cO.conditional.comparator = "=";
                                cO.conditional.value = 1;
                                cO.options = new List<optionObject>();
                                defaultConditional = false;
                                //trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Add(cO);
                            }
                            cO.jump = true;
                            cO.jumpTarget = new dialogTag(arg[0], Int32.Parse(arg[1]), arg[2]);
                            break;
                        }
                    case '=': //default go to 
                        {
                            bool exit = false;
                            if (arg[0].Length != 0)
                            {
                                if (arg[0].ToArray()[0] == '*')//exit
                                {
                                    nl = nl.Remove(0, 1);
                                    arg = nl.Split(':');
                                    exit = true;
                                }
                            }
                            if (arg[0] == "")
                            {
                                arg[0] = curtag.trunk;
                            }
                            if (arg[1] == "")
                            {
                                arg[1] = (curtag.id + 1).ToString();
                            }
                            if (arg[2] == "")
                            {
                                arg[2] = curtag.branch;
                            }
                            if (defaultConditional)
                            {
                                trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Add(new conditionalObject());
                                cO = trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Last();
                                cO.conditional = new conditionalCheck();
                                cO.conditional.Domain = "Global";
                                cO.conditional.Variable = "True";
                                cO.conditional.comparator = "=";
                                cO.conditional.value = 1;
                                cO.options = new List<optionObject>();
                                defaultConditional = false;
                                //trunks[curtag.trunk].branches[curtag.branch].entries[curtag.name()].conditionals.Add(cO);
                            }
                            cO.defaultExit = exit;
                            cO.defaultTag = new dialogTag(arg[0], Int32.Parse(arg[1]), arg[2]);
                            break;
                        }
                    case '-': //clear conditional
                        {
                            defaultConditional = true;
                            break;
                        }
                    default:
                        break;
                }
            } 
 


            return false;
        }


        public bool save_file(ref StreamWriter output)
        {
            foreach(string t in trunks.Keys)
            {
                foreach(string b in trunks[t].branches.Keys)
                {
                    foreach(string e in trunks[t].branches[b].entries.Keys)
                    {
                        output.WriteLine(
                            "!" + t + ":" + trunks[t].branches[b].entries[e].tag.id.ToString() + ":" + b);
                        for (int i = 0; i < trunks[t].branches[b].entries[e].portraits.Count; ++i)
                        {

                            output.Write("$");
                            output.Write(trunks[t].branches[b].entries[e].portraits[i].character + ":");
                            output.Write(trunks[t].branches[b].entries[e].portraits[i].pose + ":");
                            output.Write(trunks[t].branches[b].entries[e].portraits[i].side + ":");
                            output.Write(trunks[t].branches[b].entries[e].portraits[i].offset.ToString() + "\r\n");
                        }
                        for (int i = 0; i < trunks[t].branches[b].entries[e].voicelines.Count; ++i)
                        {

                            output.Write("#");
                            output.Write(trunks[t].branches[b].entries[e].voicelines[i].character + ":");
                            output.Write(trunks[t].branches[b].entries[e].voicelines[i].voiceline + ":");
                            output.Write(trunks[t].branches[b].entries[e].voicelines[i].volume.ToString() + "\r\n");
                        }
                        for (int i = 0; i < trunks[t].branches[b].entries[e].scripts.Count; ++i)
                        {

                            output.Write("%");
                            output.Write(trunks[t].branches[b].entries[e].scripts[i].domain + ":");
                            output.Write(trunks[t].branches[b].entries[e].scripts[i].script + ":");
                            foreach(string a in trunks[t].branches[b].entries[e].scripts[i].values)
                                output.Write(a + ":");
                            output.Write("\r\n");
                        }

                        output.WriteLine(
                            "~" + trunks[t].branches[b].entries[e].defaultText.character + ":" +
                            trunks[t].branches[b].entries[e].defaultText.nameid + ":" +
                            trunks[t].branches[b].entries[e].defaultText.text);
                        if (trunks[t].branches[b].entries[e].defaultOptions.speed != 100) 
                            output.WriteLine("*Speed:" + trunks[t].branches[b].entries[e].defaultOptions.speed);

                        for (int i = 0; i < trunks[t].branches[b].entries[e].conditionals.Count; ++i)
                        {

                            output.Write("@");
                            output.Write(trunks[t].branches[b].entries[e].conditionals[i].conditional.Domain + ":");
                            output.Write(trunks[t].branches[b].entries[e].conditionals[i].conditional.Variable + ":");
                            output.Write(trunks[t].branches[b].entries[e].conditionals[i].conditional.comparator + ":");
                            output.Write(trunks[t].branches[b].entries[e].conditionals[i].conditional.value.ToString() + "\r\n");

                            if (trunks[t].branches[b].entries[e].conditionals[i].options.Count > 0)
                            {
                                for (int l = 0; l < trunks[t].branches[b].entries[e].conditionals[i].options.Count; ++l)
                                {
                                    output.Write("&");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].options[l].exitDialog)
                                        output.Write("*");

                                    if (trunks[t].branches[b].entries[e].conditionals[i].options[l].tag.trunk != t)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].options[l].tag.trunk);
                                    output.Write(":");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].options[l].tag.id != trunks[t].branches[b].entries[e].tag.id + 1)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].options[l].tag.id.ToString());
                                    output.Write(":");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].options[l].tag.branch != b)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].options[l].tag.branch);
                                    output.Write(":");
                                    output.Write(trunks[t].branches[b].entries[e].conditionals[i].options[l].optionDialog+ "\r\n");
                                }
                            }
                            else
                            {
                                if (!trunks[t].branches[b].entries[e].conditionals[i].jump)
                                {
                                    output.Write("=");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].defaultExit)
                                        output.Write("*");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].defaultTag.trunk != t)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].defaultTag.trunk);
                                    output.Write(":");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].defaultTag.id != trunks[t].branches[b].entries[e].tag.id + 1)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].defaultTag.id.ToString());
                                    output.Write(":");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].defaultTag.branch != b)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].defaultTag.branch);
                                    output.Write("\r\n");
                                }
                                else
                                {
                                    output.Write(">");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].jumpTarget.trunk != t)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].jumpTarget.trunk);
                                    output.Write(":");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].jumpTarget.id != trunks[t].branches[b].entries[e].tag.id + 1)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].jumpTarget.id.ToString());
                                    output.Write(":");
                                    if (trunks[t].branches[b].entries[e].conditionals[i].jumpTarget.branch != b)
                                        output.Write(trunks[t].branches[b].entries[e].conditionals[i].jumpTarget.branch);
                                    output.Write("\r\n");
                                }
                            }
                        }
                    }
                }
            }
            output.Close();

            return true;
        }
        public dialogEntry findEntry(string trunk, string branch, string fullname)
        {
            //  string temp = fullname.Remove(0, trunk.Length);
            //  temp = temp.Substring(0, temp.Length - branch.Length);
            //  int id = Int32.Parse(temp);
            if (branch == "[default branch]")
                branch = "";
            if (trunks.ContainsKey(trunk))
            {
                if (trunks[trunk].branches.ContainsKey(branch))
                {
                    if (trunks[trunk].branches[branch].entries.ContainsKey(fullname))
                    {

                        return trunks[trunk].branches[branch].entries[fullname];
                    }
                }
            }
            Console.WriteLine(trunk + ":" + branch + "-" + fullname);
            return null;
        }

    }
}
