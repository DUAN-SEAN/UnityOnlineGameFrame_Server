using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ProtobufComplier
{
    
    
    internal class OpcodeInfo
    {
        public string Name;
        public int Opcode;
    }


    class Program
    {

        private const string protoPath = "../../SDModel-All/NetWorkProject/Proto/";
        private const string ServerMessagePath = "../../SDModel-All/NetWorkProject/Message";

        private const string ClientMessagePath = "../../Crazy Gun Client/Assets/SDModel/NetWorkProject/Message";

        private const string serverProtocPath = "ProtocBatServer.bat";
        private const string clientProtocPath = "ProtocBatClient.bat";
        private static readonly char[] splitChars = { ' ', '\t' };
        private static readonly List<OpcodeInfo> msgOpcode = new List<OpcodeInfo>();
        static void Main(string[] args)
        {
            Console.WriteLine("开始生成Opcode类!");

            AllProto2CS();
            Console.WriteLine("开始生成Message类！");
            //string path = args[0];


            CommandRun(serverProtocPath, "");
            CommandRun(clientProtocPath, "");
            //CommandRun($"ProtocBat", "");
           
            
            Console.WriteLine("按下结束！");
            Console.ReadKey();
        }

        public static void AllProto2CS()
        {

            
     
                msgOpcode.Clear();
                Proto2CS("SDModel", "OutMessage.proto", ServerMessagePath, "Opcode", 100);

                Proto2CS("SDModel", "OutMessage.proto", ClientMessagePath, "Opcode", 100);









        }

        public static void CommandRun(string exe, string arguments)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = exe,
                    Arguments = arguments,
                    UseShellExecute = true,
                };
                Process p = Process.Start(info);
                p.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// 生成消息类
        /// </summary>
        /// <param name="ns">程序集名</param>
        /// <param name="protoName">proto文件名</param>
        /// <param name="outputPath">输出路径</param>
        /// <param name="opcodeClassName">opcode类名</param>
        /// <param name="startOpcode">op指令起始</param>
        /// <param name="isClient">是否是客户端</param>
        public static void Proto2CS(string ns, string protoName, string outputPath, string opcodeClassName, int startOpcode, bool isClient = true)
        {
            msgOpcode.Clear();
            string proto = Path.Combine(protoPath, protoName);
            Console.Write(proto);
            //CommandRun($"protoc.exe", $"--csharp_out=\"./{outputPath}\" --proto_path=\"{protoPath}\" {protoName}");

            string s = File.ReadAllText(proto);

            StringBuilder sb = new StringBuilder();
            sb.Append("using SDModel;\n");
            sb.Append($"namespace {ns}\n");
            sb.Append("{\n");

            bool isMsgStart = false;

            foreach (string line in s.Split('\n'))
            {
                string newline = line.Trim();

                if (newline == "")
                {
                    continue;
                }

                if (newline.StartsWith("//"))
                {
                   
                    sb.Append($"{newline}\n");
                }

                if (newline.StartsWith("message"))
                {
                    string parentClass = "";
                    isMsgStart = true;
                    string msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                    if (ss.Length == 2)
                    {
                        parentClass = ss[1].Trim();
                    }
                    else
                    {
                        parentClass = "";
                    }

                    msgOpcode.Add(new OpcodeInfo() { Name = msgName, Opcode = ++startOpcode });

                    sb.Append($"\t[Message({opcodeClassName}.{msgName})]\n");
                    sb.Append($"\tpublic partial class {msgName} ");
                    if (parentClass != "")
                    {
                        sb.Append($": {parentClass} ");
                    }

                    sb.Append("{}\n\n");
                }

                if (isMsgStart && newline == "}")
                {
                    isMsgStart = false;
                }
            }
            sb.Append("}\n");

            GenerateOpcode(ns, opcodeClassName, outputPath, sb);
        }

        private static void GenerateOpcode(string ns, string outputFileName, string outputPath, StringBuilder sb)
        {
            sb.AppendLine($"namespace {ns}");          
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic static partial class {outputFileName}");
            sb.AppendLine("\t{");
            foreach (OpcodeInfo info in msgOpcode)
            {
                sb.AppendLine($"\t\t public const ushort {info.Name} = {info.Opcode};");
            }
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            string csPath = Path.Combine(outputPath, outputFileName + ".cs");
            File.WriteAllText(csPath, sb.ToString());
                        }
    }
}

