using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufComplier
{
    internal class OpcodeInfo
    {
        public string Name;
        public int Opcode;
    }
    internal class ProtoInfo
    {
        public string path;
        public string name;
    }
    /// <summary>
    /// 1 读取指定目录下后缀为.proto的文件
    /// 2 将文件都通过protoer编译成message 放入指定的目录中
    ///     需要读取protoer的地址
    /// 3 根据proto生成对应的opcode，注意proto的ID要从第一个proto文件到最后一个文件 不能有重复的
    ///     opcode文件要包含namespace 以及引用的命名空间
    /// </summary>
    class Program
    {      
        private static readonly char[] splitChars = { ' ', '\t' };
        private static readonly List<ProtoInfo> protos =new List<ProtoInfo>();
        private static readonly List<OpcodeInfo> msgOpcode = new List<OpcodeInfo>();
        /// <summary>
        /// args:
        /// 0 protoer 目录
        /// 1 proto 目录
        /// 2 message 目录
        /// 3 opcode namespace 
        /// 4 opcode using 
        /// 5 opcode index
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            Init(args[1]);

            Start(args[1],args[3], args[2],int.Parse(args[5]));

            ProtoWork(args[0],args[2],args[1]);

            Console.WriteLine("按下结束！");

            Console.ReadKey();
        }
        
        /// <summary>
        /// 进行proto初始化
        /// </summary>
        /// <param name="protoerPath"></param>
        public static void ProtoWork(string protoerPath,string messagePath,string protoPath)
        {
            Console.WriteLine("ProtoWork::开始进行proto解析");
            foreach (var item in protos)
            {

                //protoc.exe --csharp_out="../../Crazy Gun Client/Assets/SDModel/NetWorkProject/Message" --proto_path="../../SDModel-All/NetWorkProject/Proto" OutMessage.proto
                string command = String.Format("{0} --csharp_out=\"{1}\" --proto_path=\"{2}\" {3}", protoerPath, messagePath,protoPath,item.name);
                Console.WriteLine("ProtoWork::Command = " + command);
                bool bError = false;
                var ts = Task.Run(() =>
                {
                    var ret = ExecuteCmd("cmd.exe", "/C" + command, 0, out bError);
                    Console.WriteLine(ret);
                });
                ts.Wait();
                Console.WriteLine($"ProtoWork::生成{item.name}成功!");
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="protoPath"></param>
        public static void Init(string protoPath)
        {
            protos.Clear();
            var protoArray = Directory.GetFiles(protoPath, "*.proto", SearchOption.TopDirectoryOnly);
            if (protoArray == null || protoArray.Length == 0)
            {
                Console.WriteLine("InitProto::读取proto路径为null或proto为空");
                return;
            }
            foreach (var item in protoArray)
            {
                string filename = Path.GetFileName(item);
                
                protos.Add(new ProtoInfo { path = item,name = filename});
            }
            Console.WriteLine($"Init::proto个数为{protos.Count}");
        }
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="ns">命名空间</param>
        /// <param name="outMessagePath">输出路径</param>
        /// <param name="index"></param>
        public static void Start(string protoPath,string ns,string outMessagePath,int index = 100)
        {
            Console.WriteLine("Start::开始生成Opcode");
            foreach (var item in protos)
            {
                string opcodeName = Path.GetFileNameWithoutExtension(item.path) + "Opcode";
                Proto2CS(protoPath,ns, item.name, outMessagePath, opcodeName, ref index);
            }
        }

        /// <summary>  
        /// 执行DOS命令，返回DOS命令的输出  
        /// </summary>  
        /// <param name="dosCommand">dos命令</param>  
        /// <param name="milliseconds">等待命令执行的时间（单位：毫秒），  
        /// 如果设定为0，则无限等待</param>  
        /// <param name="bError">是否输出了错误流</param>  
        /// <returns>返回DOS命令的输出</returns>  
        public static string ExecuteCmd(string runCom, string command, int seconds, out bool bError)
        {
            bError = false;
            string outputAndError = ""; //输出字符串  
            if (command != null && !command.Equals(""))
            {
                Process process = new Process();//创建进程对象  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = runCom;//设定需要执行的命令  
                startInfo.Arguments = command;//“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动  
                startInfo.RedirectStandardInput = false;//不重定向输入  
                startInfo.RedirectStandardOutput = true; //重定向输出  
                startInfo.RedirectStandardError = true; //重定向输出  
                startInfo.CreateNoWindow = true;//不创建窗口  
                process.StartInfo = startInfo;
                try
                {
                   
                    if (process.Start())//开始进程  
                    {
                        
                        outputAndError = process.StandardOutput.ReadToEnd(); //读取进程的输出  
                        //process.BeginOutputReadLine();
                        
                        var errorMsg = process.StandardError.ReadToEndAsync().Result;
                        if (errorMsg.Length != 0)
                        {
                            outputAndError += errorMsg; //读取进程的输出  
                            bError = true;
                        }
                        
                        if (seconds == 0)
                        {
                            
                            process.WaitForExit();//这里无限等待进程结束  
                        }
                        else
                        {
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒  
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("{0}", ex.Message);
                }
                finally
                {
                    if (process != null)
                    {
                        
                        process.Close();
                    }
                }
            }
            
            return outputAndError;
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
        public static void Proto2CS(string protoPath,string ns, string protoName, string outputPath, string opcodeClassName, ref int startOpcode, bool isClient = true)
        {

            msgOpcode.Clear();
            string proto = Path.Combine(protoPath, protoName);
            //Console.Write(proto);
            //CommandRun($"protoc.exe", $"--csharp_out=\"./{outputPath}\" --proto_path=\"{protoPath}\" {protoName}");

            string s = File.ReadAllText(proto);

            StringBuilder sb = new StringBuilder();
            sb.Append($"using {ns};\n");
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

