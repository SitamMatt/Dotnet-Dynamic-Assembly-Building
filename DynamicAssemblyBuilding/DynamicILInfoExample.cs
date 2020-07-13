using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace DynamicAssemblyBuilding
{
    public class DynamicILInfoExample
    {
        public static void Run()
        {
            // IL code to execute 
            byte[] code = {
                0, //     IL_0000: nop
                27,//     IL_0001: ldc.i4.5
                10,//     IL_0002: stloc.0
                31,//     IL_0003: ldc.i4.s 10
                10,//     IL_0004: 10
                11,//     IL_0005: stloc.1
                6, //     IL_0006: ldloc.0
                7, //     IL_0007: ldloc.1
                88,//     IL_0008: add
                12,//     IL_0009: stloc.2
                43,//     IL_000a: br.s     
                0, //     IL_000b: 0
                8, //     IL_000c: ldloc.2
                42 //     IL_000d: ret
            };
            // the code above is equal to the following method body
            //            int a = 5;
            //            int c = 10;
            //            return a+c;
            var dynamicMethod = new DynamicMethod("", typeof(int), new Type[0]);
            var ilInfo = dynamicMethod.GetDynamicILInfo();
            // local variables signature helper
            var signatureHelper = SignatureHelper.GetLocalVarSigHelper();
            signatureHelper.AddArgument(typeof(int)); // 1st local variable a
            signatureHelper.AddArgument(typeof(int)); // 2nd local variable c
            signatureHelper.AddArgument(typeof(int)); // 3rd local variable - result of {a+c}
            // assembly dynamic info setup
            ilInfo.SetCode(code, 2); // minimal stack size for this code is 2
            ilInfo.SetLocalSignature(signatureHelper.GetSignature());
            // create delegate
            var dynamicDelegate = (Func<int>)dynamicMethod.CreateDelegate(typeof(Func<int>));

            var result = dynamicDelegate();
            
            Debug.Assert(result == 20); // result should be 15
        }
    }
}