//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年03月03日 2:04:29
//DLRTest

using Microsoft.Scripting.Hosting;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Icarus.DLR.Test
{
    public class DLRTest : MonoBehaviour
    {
        public InputField InputField;
        public Button ButtonObj;
        public Image Image;

        private string PyName = "tt.py";

        private string ConfigName = "Script";

        void Start()
        {
            ButtonObj.gameObject.SetActive(false);
            
            var config = Resources.Load<TextAsset>(ConfigName).bytes;
            _setup = ScriptRuntimeSetup.ReadConfiguration(new MemoryStream(config));
            _ex += "\n 配置加载结束 --- IronPython 版本:" + _setup.LanguageSetups[0].DisplayName;
            
            StartCoroutine(_startCopy());
        }
        
        private string _ex;
        private dynamic _script;
        private ScriptRuntimeSetup _setup;
        private ScriptRuntime _scriptruntim;
        private ScriptSource _source;
        private Assembly _assembly;
        /// <summary>
        /// 主要函数 --- 哔哔哔
        /// </summary>
        public void StartScript()
        {
            Debug.LogError(Application.persistentDataPath);
            
            try
            {
                _scriptruntim = ScriptRuntime.CreateRemote(AppDomain.CurrentDomain, _setup);
                _ex += "\n 创建ScriptRunTime成功";
            }
            catch (Exception e)
            {
                _ex += "\n 创建ScriptRuntime异常,ex:" + e;
                throw;
            }
            finally
            {
                InputField.text = _ex;
            }
            //            try
            //            {
            //                //_assembly = Assembly.GetAssembly(typeof(GameObject));
            //                //_scriptruntim.LoadAssembly(_assembly);
            //                //_ex += "\n 加载UnityEngine到ScriptRunTime成功,路径:\n \t" + _assembly.CodeBase;
            //
            //                _assembly = Assembly.GetAssembly(typeof(tt));
            //                _scriptruntim.LoadAssembly(_assembly);
            //                
            //                _ex += "\n 加载Icarus.DLR.Test到ScriptRunTime成功,路径:\n \t" + _assembly.CodeBase;
            //
            //            }
            //            catch (Exception e)
            //            {
            //                _ex += "\n 加载UnityEngine到ScriptRunTime失败,ex:"+e;
            //            }
            //            finally
            //            {
            //                InputField.text = _ex;
            //            }
            try
            {
                _script = _scriptruntim.UseFile(_getPyPath(_getPersistentDataPath()));
                _ex += "\n 加载脚本成功";
            }
            catch (Exception e)
            {
                _ex += "\n 打开脚本出现错误:" + e;
                throw;
            }
            finally
            {
                InputField.text = _ex;
            }
            try
            {
                _ex += "\n 获取变量 test:" + _script.test;
                _ex += "\n 脚本执行结束";
                ButtonObj.gameObject.SetActive(true);
                foreach (var ces in _script.clr.References)
                {
                    _ex += "\n 程序集:"+ces;
                }
                
            }
            catch (Exception e)
            {
                _ex += "\n 执行脚本出现错误:" + e;
                throw;
            }
            finally
            {
                InputField.text = _ex;
            }

        }

        /// <summary>
        /// 调用Py脚本函数设置颜色
        /// </summary>
        public void SetColor()
        {
            try
            {
                var color = _script.SetColor(Image,
                    new Color(Random.Range(0, 1.1f), Random.Range(0, 1.1f),
                        Random.Range(0, 1.1f), Random.Range(0, 1.1f)));
                InputField.text = "SetColor函数返回值:" + color.ToString();
            }
            catch (Exception e)
            {
                InputField.text = "调用SetColor函数出错:" + e;
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            _ex = null;
            InputField.text = null;
        }

        #region Private function

        private IEnumerator _startCopy()
        {
            yield return StartCoroutine(_copy(_getPyPath(_getStreamingAssetsPath()), _getPyPath(_getPersistentDataPath())));
            StartScript();
        }

        private string _getPersistentDataPath()
        {
            return Application.persistentDataPath;
        }

        private string _getStreamingAssetsPath()
        {
            return Application.streamingAssetsPath;
        }
        
        private string _getPyPath(string root)
        {
            return Path.Combine(root, PyName);
        }

        private IEnumerator _copy(string oldPath, string newPath)
        {
            _ex += "\n Copy Start";
            string src = oldPath;
            string des = newPath;
            _ex += "\n oldPath:" + des;
            _ex += "\n newPath:" + src;
            WWW www = new WWW(src);
            while (!www.isDone)
            {
                _ex += "\n Copy 进度:" + www.progress;
                yield return null;
            }
            if (!string.IsNullOrEmpty(www.error))
            {
                _ex += "\n Copy失败:" + www.error;
            }
            else
            {
                if (File.Exists(des))
                {
                    _ex += "\n 文件存在,跳过";
                    yield break;
                }
                FileStream fsDes = File.Create(des);
                _ex += "\n 文件创建完成,开始写入";
                fsDes.Write(www.bytes, 0, www.bytes.Length);
                _ex += "\n 写入完成";
                fsDes.Flush();
                fsDes.Close();
                _ex += "\n Copy完成,路径:" + des;
            }
            www.Dispose();
        }

        #endregion

        

    }

    public class tt
    {
        public static void Test(Image image)
        {
            image.color = Color.red;
        }
    }
}
