/*
 * ------------------------------------------------------------------------
 * 命名空间 : RsaUtil
 *
 * 文件名  : RSAUtil.cs 
 * 
 * 作用    : 提供RSA加密，解密（需配合秘钥生成工具，秘钥生成工具以控制台应用程样式给出）
 * 
 * 时间    : 2016/12/29
 * 
 * 是否线程安全 : 否
 *
 * ------------------------------------------------------------------------
 * 说明 :
 * 
 * 由于.net平台对于Rsa加密中导出秘钥的 API 只支持两种
 * 
 * 1 导出到XML文件 
 * 2 导出到秘钥容器
 * 本工具支持这两种方式读取秘钥
 * 
 * 对于由其他语言进行RSA加密可能是直接给出秘钥的字符串形式，这里就不适用了
 *
 * 需要在SSO服务上添加新的功能。
 * -----------------------------------------------------------------------
 * 变更记录:
 * 
 *  变更说明                    时间                 操作人          具体项
 *  创建文件                    2016/12/29          Neo
 *  添加签名操作                 2017/1/11           Neo            添加函数:HashAndSign VerifyHash
 *-----------------------------------------------------------------------*/
using System;
using System.Security.Cryptography;
using System.IO;

/// <summary>
/// Rsa 加密,解密
/// </summary>
namespace RsaUtil
{
    class RSAUtil
    {
        private const int DWKEYSIZE = 1024;

        /// <summary>
        /// 判断加密原文长度是否超出限制
        /// </summary>
        /// <param name="source">需加密的原文</param>
        /// <returns>true 可加密  false不可加密</returns>
        public static bool CheckSourceValidate(string source)
        {
            return (DWKEYSIZE / 8 - 11) >= source.Length;
        }

        /// <summary>
        /// 从Xml文件中获取RSA公钥
        /// </summary>
        /// <param name="xmlfile">公钥文件路径</param>
        /// <returns>秘钥参数 RSAParamters / 无法获取返回null </returns>
        public static RSAParameters GetPublicKeyFromXml(string xmlfile)
        {
            using (StreamReader reader = new StreamReader(xmlfile))
            {
                string keystr = reader.ReadToEnd();
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(keystr);

                return rsa.ExportParameters(false);
            }
        }

        /// <summary>
        /// 从容器中获取RSA秘钥
        /// </summary>
        /// <param name="container">秘钥容器名字</param>
        /// <returns>秘钥参数 RSAParamters / 无法获取返回null</returns>
        public static RSAParameters GetPrivateKeyFromCantain(string container)
        {
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = container;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

            return rsa.ExportParameters(true);
        }

        /// <summary>
        /// 从XML文件中获取RSA秘钥
        /// </summary>
        /// <param name="xmlfile">包含私钥的xml文件</param>
        /// <returns>秘钥参数 RSAParamters / 无法获取返回null</returns>
        public static RSAParameters GetPrivateKeyFromXml(string xmlfile)
        {
            using (StreamReader reader = new StreamReader(xmlfile))
            {
                string keystr = reader.ReadToEnd();
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(keystr);

                return rsa.ExportParameters(true);
            }
        }

        /// <summary>
        /// 加密操作
        /// </summary>
        /// <param name="DataToEncrypt">加密数据</param>
        /// <param name="RSAKeyInfo">包含秘钥信息的RsaParamter对象</param>
        /// <param name="DoOAEPPadding">针对XP系统特殊设置,如再XP上自习 传True</param>
        /// <returns></returns>
        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {

            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

                RSA.ImportParameters(RSAKeyInfo);

                return RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        /// <summary>
        /// 解密操作
        /// </summary>
        /// <param name="DataToDecrypt"></param>
        /// <param name="RSAKeyInfo"></param>
        /// <param name="DoOAEPPadding"></param>
        /// <returns></returns>
        public static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

                RSA.ImportParameters(RSAKeyInfo);

                return RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }

        }

        /// <summary>
        /// 使用SHA256进行数据签名
        /// </summary>
        /// <param name="encrypted">需进行Hash签名的数据</param>
        /// <param name="rsaPrivateParams">签名秘钥(即RSA私钥)</param>
        /// <returns>byte[]签名后的数据</returns>
        public static byte[] HashAndSign(byte[] encrypted, RSAParameters rsaPrivateParams)
        {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA256Managed hash = new SHA256Managed();
            byte[] hashedData;

            rsaCSP.ImportParameters(rsaPrivateParams);

            hashedData = hash.ComputeHash(encrypted);
            return rsaCSP.SignHash(hashedData, CryptoConfig.MapNameToOID("SHA256"));
        }


        /// <summary>
        /// 验证签名的正确性,是否经过修改
        /// </summary>
        /// <param name="rsaParams">进行签名解密的秘钥(即RSA公钥)</param>
        /// <param name="signedData">签名前的原始数据</param>
        /// <param name="signature">签名后的数据</param>
        /// <returns>true:签名有效  false:签名无效</returns>
        public static bool VerifyHash(RSAParameters rsaParams, byte[] signedData, byte[] signature)
        {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA256Managed hash = new SHA256Managed();
            byte[] hashedData;

            rsaCSP.ImportParameters(rsaParams);

            hashedData = hash.ComputeHash(signedData);
            return rsaCSP.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA256"), signature);
        }

    }
}
