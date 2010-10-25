// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GenericProcessorTests
    {
        #region Description

        [TestMethod]
        [Description("Processor creates its input arguments correctly")]
        public void Processor_Builds_InputArguments()
        {
            MockProcessor1 mock = new MockProcessor1();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("intValue", args[0].Name, "Wrong input argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input argument type");
        }

        [TestMethod]
        [Description("Processor creates its output arguments correctly")]
        public void Processor_Builds_OutputArguments()
        {
            MockProcessor1 mock = new MockProcessor1();
            ProcessorArgumentCollection args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockProcessor1Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");
        }

        [TestMethod]
        [Description("Processor creates its argument descriptions correctly for a derived type")]
        public void Processor_Builds_InputArguments_Derived()
        {
            MockProcessor1Derived mock = new MockProcessor1Derived();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("intValueDerived", args[0].Name, "Wrong input argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input argument type");
        }

        [TestMethod]
        [Description("Processor generic with 2 parameters")]
        public void Processor_Generic2_Builds_Correctly()
        {
            MockGeneric2 mock = new MockGeneric2();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input", args[0].Name, "Wrong input argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric2Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1 });
        }

        [TestMethod]
        [Description("Processor generic with 3 parameters")]
        public void Processor_Generic3_Builds_Correctly()
        {
            MockGeneric3 mock = new MockGeneric3();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(2, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric3Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint) 2 });
        }

        [TestMethod]
        [Description("Processor generic with 4 parameters")]
        public void Processor_Generic4_Builds_Correctly()
        {
            MockGeneric4 mock = new MockGeneric4();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(3, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric4Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1,(uint) 2, (short) 3 });
        }

        [TestMethod]
        [Description("Processor generic with 5 parameters")]
        public void Processor_Generic5_Builds_Correctly()
        {
            MockGeneric5 mock = new MockGeneric5();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(4, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric5Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort) 4});
        }

        [TestMethod]
        [Description("Processor generic with 6 parameters")]
        public void Processor_Generic6_Builds_Correctly()
        {
            MockGeneric6 mock = new MockGeneric6();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(5, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric6Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5 });
        }

        [TestMethod]
        [Description("Processor generic with 7 parameters")]
        public void Processor_Generic7_Builds_Correctly()
        {
            MockGeneric7 mock = new MockGeneric7();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(6, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric7Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6 });
        }

        [TestMethod]
        [Description("Processor generic with 8 parameters")]
        public void Processor_Generic8_Builds_Correctly()
        {
            MockGeneric8 mock = new MockGeneric8();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(7, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric8Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7 });
        }

        [TestMethod]
        [Description("Processor generic with 9 parameters")]
        public void Processor_Generic9_Builds_Correctly()
        {
            MockGeneric9 mock = new MockGeneric9();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(8, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric9Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now });
        }

        [TestMethod]
        [Description("Processor generic with 10 parameters")]
        public void Processor_Generic10_Builds_Correctly()
        {
            MockGeneric10 mock = new MockGeneric10();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(9, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");
            Assert.AreEqual("input9", args[8].Name, "Wrong input[8] argument name");
            Assert.AreEqual(typeof(Uri), args[8].ArgumentType, "Wrong input[8] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric10Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now, new Uri("http://host") });
        }

        [TestMethod]
        [Description("Processor generic with 11 parameters")]
        public void Processor_Generic11_Builds_Correctly()
        {
            MockGeneric11 mock = new MockGeneric11();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(10, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");
            Assert.AreEqual("input9", args[8].Name, "Wrong input[8] argument name");
            Assert.AreEqual(typeof(Uri), args[8].ArgumentType, "Wrong input[8] argument type");
            Assert.AreEqual("input10", args[9].Name, "Wrong input[9] argument name");
            Assert.AreEqual(typeof(string), args[9].ArgumentType, "Wrong input[9] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric11Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now, new Uri("http://host"), "hello" });
        }

        [TestMethod]
        [Description("Processor generic with 12 parameters")]
        public void Processor_Generic12_Builds_Correctly()
        {
            MockGeneric12 mock = new MockGeneric12();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(11, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");
            Assert.AreEqual("input9", args[8].Name, "Wrong input[8] argument name");
            Assert.AreEqual(typeof(Uri), args[8].ArgumentType, "Wrong input[8] argument type");
            Assert.AreEqual("input10", args[9].Name, "Wrong input[9] argument name");
            Assert.AreEqual(typeof(string), args[9].ArgumentType, "Wrong input[9] argument type");
            Assert.AreEqual("input11", args[10].Name, "Wrong input[10] argument name");
            Assert.AreEqual(typeof(int), args[10].ArgumentType, "Wrong input[10] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric12Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now, new Uri("http://host"), "hello", 11 });
        }

        [TestMethod]
        [Description("Processor generic with 13 parameters")]
        public void Processor_Generic13_Builds_Correctly()
        {
            MockGeneric13 mock = new MockGeneric13();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(12, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");
            Assert.AreEqual("input9", args[8].Name, "Wrong input[8] argument name");
            Assert.AreEqual(typeof(Uri), args[8].ArgumentType, "Wrong input[8] argument type");
            Assert.AreEqual("input10", args[9].Name, "Wrong input[9] argument name");
            Assert.AreEqual(typeof(string), args[9].ArgumentType, "Wrong input[9] argument type");
            Assert.AreEqual("input11", args[10].Name, "Wrong input[10] argument name");
            Assert.AreEqual(typeof(int), args[10].ArgumentType, "Wrong input[10] argument type");
            Assert.AreEqual("input12", args[11].Name, "Wrong input[11] argument name");
            Assert.AreEqual(typeof(uint), args[11].ArgumentType, "Wrong input[11] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric13Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now, new Uri("http://host"), "hello", 11, (uint)12 });
        }

        [TestMethod]
        [Description("Processor generic with 14 parameters")]
        public void Processor_Generic14_Builds_Correctly()
        {
            MockGeneric14 mock = new MockGeneric14();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(13, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");
            Assert.AreEqual("input9", args[8].Name, "Wrong input[8] argument name");
            Assert.AreEqual(typeof(Uri), args[8].ArgumentType, "Wrong input[8] argument type");
            Assert.AreEqual("input10", args[9].Name, "Wrong input[9] argument name");
            Assert.AreEqual(typeof(string), args[9].ArgumentType, "Wrong input[9] argument type");
            Assert.AreEqual("input11", args[10].Name, "Wrong input[10] argument name");
            Assert.AreEqual(typeof(int), args[10].ArgumentType, "Wrong input[10] argument type");
            Assert.AreEqual("input12", args[11].Name, "Wrong input[11] argument name");
            Assert.AreEqual(typeof(uint), args[11].ArgumentType, "Wrong input[11] argument type");
            Assert.AreEqual("input13", args[12].Name, "Wrong input[12] argument name");
            Assert.AreEqual(typeof(short), args[12].ArgumentType, "Wrong input[12] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric14Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now, new Uri("http://host"), "hello", 11, (uint)12, (short)13 });
        }

        [TestMethod]
        [Description("Processor generic with 15 parameters")]
        public void Processor_Generic15_Builds_Correctly()
        {
            MockGeneric15 mock = new MockGeneric15();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(14, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");
            Assert.AreEqual("input9", args[8].Name, "Wrong input[8] argument name");
            Assert.AreEqual(typeof(Uri), args[8].ArgumentType, "Wrong input[8] argument type");
            Assert.AreEqual("input10", args[9].Name, "Wrong input[9] argument name");
            Assert.AreEqual(typeof(string), args[9].ArgumentType, "Wrong input[9] argument type");
            Assert.AreEqual("input11", args[10].Name, "Wrong input[10] argument name");
            Assert.AreEqual(typeof(int), args[10].ArgumentType, "Wrong input[10] argument type");
            Assert.AreEqual("input12", args[11].Name, "Wrong input[11] argument name");
            Assert.AreEqual(typeof(uint), args[11].ArgumentType, "Wrong input[11] argument type");
            Assert.AreEqual("input13", args[12].Name, "Wrong input[12] argument name");
            Assert.AreEqual(typeof(short), args[12].ArgumentType, "Wrong input[12] argument type");
            Assert.AreEqual("input14", args[13].Name, "Wrong input[13] argument name");
            Assert.AreEqual(typeof(ushort), args[13].ArgumentType, "Wrong input[13] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric15Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now, new Uri("http://host"), "hello", 11, (uint)12, (short)13, (ushort)14 });
        }

        [TestMethod]
        [Description("Processor generic with 16 parameters")]
        public void Processor_Generic16_Builds_Correctly()
        {
            MockGeneric16 mock = new MockGeneric16();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(15, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");
            Assert.AreEqual("input9", args[8].Name, "Wrong input[8] argument name");
            Assert.AreEqual(typeof(Uri), args[8].ArgumentType, "Wrong input[8] argument type");
            Assert.AreEqual("input10", args[9].Name, "Wrong input[9] argument name");
            Assert.AreEqual(typeof(string), args[9].ArgumentType, "Wrong input[9] argument type");
            Assert.AreEqual("input11", args[10].Name, "Wrong input[10] argument name");
            Assert.AreEqual(typeof(int), args[10].ArgumentType, "Wrong input[10] argument type");
            Assert.AreEqual("input12", args[11].Name, "Wrong input[11] argument name");
            Assert.AreEqual(typeof(uint), args[11].ArgumentType, "Wrong input[11] argument type");
            Assert.AreEqual("input13", args[12].Name, "Wrong input[12] argument name");
            Assert.AreEqual(typeof(short), args[12].ArgumentType, "Wrong input[12] argument type");
            Assert.AreEqual("input14", args[13].Name, "Wrong input[13] argument name");
            Assert.AreEqual(typeof(ushort), args[13].ArgumentType, "Wrong input[13] argument type");
            Assert.AreEqual("input15", args[14].Name, "Wrong input[14] argument name");
            Assert.AreEqual(typeof(long), args[14].ArgumentType, "Wrong input[14] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric16Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now, new Uri("http://host"), "hello", 11, (uint)12, (short)13, (ushort)14, (long)15 });
        }

        [TestMethod]
        [Description("Processor generic with 17 parameters")]
        public void Processor_Generic17_Builds_Correctly()
        {
            MockGeneric17 mock = new MockGeneric17();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(16, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("input1", args[0].Name, "Wrong input[0] argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input[0] argument type");
            Assert.AreEqual("input2", args[1].Name, "Wrong input[1] argument name");
            Assert.AreEqual(typeof(uint), args[1].ArgumentType, "Wrong input[1] argument type");
            Assert.AreEqual("input3", args[2].Name, "Wrong input[2] argument name");
            Assert.AreEqual(typeof(short), args[2].ArgumentType, "Wrong input[2] argument type");
            Assert.AreEqual("input4", args[3].Name, "Wrong input[3] argument name");
            Assert.AreEqual(typeof(ushort), args[3].ArgumentType, "Wrong input[3] argument type");
            Assert.AreEqual("input5", args[4].Name, "Wrong input[4] argument name");
            Assert.AreEqual(typeof(long), args[4].ArgumentType, "Wrong input[4] argument type");
            Assert.AreEqual("input6", args[5].Name, "Wrong input[5] argument name");
            Assert.AreEqual(typeof(double), args[5].ArgumentType, "Wrong input[5] argument type");
            Assert.AreEqual("input7", args[6].Name, "Wrong input[6] argument name");
            Assert.AreEqual(typeof(float), args[6].ArgumentType, "Wrong input[6] argument type");
            Assert.AreEqual("input8", args[7].Name, "Wrong input[7] argument name");
            Assert.AreEqual(typeof(DateTime), args[7].ArgumentType, "Wrong input[7] argument type");
            Assert.AreEqual("input9", args[8].Name, "Wrong input[8] argument name");
            Assert.AreEqual(typeof(Uri), args[8].ArgumentType, "Wrong input[8] argument type");
            Assert.AreEqual("input10", args[9].Name, "Wrong input[9] argument name");
            Assert.AreEqual(typeof(string), args[9].ArgumentType, "Wrong input[9] argument type");
            Assert.AreEqual("input11", args[10].Name, "Wrong input[10] argument name");
            Assert.AreEqual(typeof(int), args[10].ArgumentType, "Wrong input[10] argument type");
            Assert.AreEqual("input12", args[11].Name, "Wrong input[11] argument name");
            Assert.AreEqual(typeof(uint), args[11].ArgumentType, "Wrong input[11] argument type");
            Assert.AreEqual("input13", args[12].Name, "Wrong input[12] argument name");
            Assert.AreEqual(typeof(short), args[12].ArgumentType, "Wrong input[12] argument type");
            Assert.AreEqual("input14", args[13].Name, "Wrong input[13] argument name");
            Assert.AreEqual(typeof(ushort), args[13].ArgumentType, "Wrong input[13] argument type");
            Assert.AreEqual("input15", args[14].Name, "Wrong input[14] argument name");
            Assert.AreEqual(typeof(long), args[14].ArgumentType, "Wrong input[14] argument type");
            Assert.AreEqual("input16", args[15].Name, "Wrong input[15] argument name");
            Assert.AreEqual(typeof(double), args[15].ArgumentType, "Wrong input[15] argument type");

            args = mock.OutArguments;
            Assert.IsNotNull(args, "Output arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("MockGeneric17Result", args[0].Name, "Wrong ouput argument name.");
            Assert.AreEqual(typeof(string), args[0].ArgumentType, "Wrong output argument type");

            mock.Initialize();
            mock.Execute(new object[] { 1, (uint)2, (short)3, (ushort)4, (long)5, (double)6, (float)7, DateTime.Now, new Uri("http://host"), "hello", 11, (uint)12, (short)13, (ushort)14, (long)15, (double)16 });
        }

        [TestMethod]
        [Description("Processor Reflection builder can handle missing OnExecute method")]
        public void Processor_ReflectionBuilder_No_OnExecute()
        {
            ProcessorWithNoExecute mock = new ProcessorWithNoExecute();
            ProcessorArgumentCollection args = mock.InArguments;
            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(0, args.Count, "Wrong number of input arguments");
        }

        [TestMethod]
        [Description("Processor Reflection builder can handle OnExecute with wrong return type")]
        public void Processor_ReflectionBuilder_Wrong_Return_Type()
        {
            ProcessorWithWrongExecuteReturn mock = new ProcessorWithWrongExecuteReturn();
            ProcessorArgumentCollection args = mock.InArguments;
            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(0, args.Count, "Wrong number of input arguments");
        }

        [TestMethod]
        [Description("Processor Reflection builder can handle OnExecute with wrong return type generic parameter")]
        public void Processor_ReflectionBuilder_Wrong_Return_Type_Generic()
        {
            ProcessorWithWrongExecuteReturnGeneric mock = new ProcessorWithWrongExecuteReturnGeneric();
            ProcessorArgumentCollection args = mock.InArguments;
            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(0, args.Count, "Wrong number of input arguments");
        }

        [TestMethod]
        [Description("Processor Reflection builder can handle OnExecute with wrong parameter type")]
        public void Processor_ReflectionBuilder_Wrong_Parameter_Type()
        {
            ProcessorWithWrongExecuteParameterType mock = new ProcessorWithWrongExecuteParameterType();
            ProcessorArgumentCollection args = mock.InArguments;
            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(0, args.Count, "Wrong number of input arguments");
        }


        [TestMethod]
        [Description("Processor Reflection builder can handle OnExecute with wrong parameter count")]
        public void Processor_ReflectionBuilder_Wrong_Parameter_Count()
        {
            ProcessorWithWrongExecuteParameterCount mock = new ProcessorWithWrongExecuteParameterCount();
            ProcessorArgumentCollection args = mock.InArguments;
            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(0, args.Count, "Wrong number of input arguments");
        }

        [TestMethod]
        [Description("Processor Reflection builder finds correct OnExecute despite presence of illegal forms")]
        public void Processor_Reflection_Builder_Valid_And_Invalid_Executes()
        {
            ProcessorWithValidAndInvalidExecute mock = new ProcessorWithValidAndInvalidExecute();
            ProcessorArgumentCollection args = mock.InArguments;

            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(1, args.Count, "Wrong number of input arguments");
            Assert.AreEqual("intValue", args[0].Name, "Wrong input argument name");
            Assert.AreEqual(typeof(int), args[0].ArgumentType, "Wrong input argument type");
        }

        [TestMethod]
        [Description("Processor Reflection builder can handle non generic class")]
        public void Processor_ReflectionBuilder_NonGeneric_Class()
        {
            MockNonGenericProcessor mock = new MockNonGenericProcessor();
            ProcessorArgumentCollection args = mock.InArguments;
            Assert.IsNotNull(args, "Input arguments cannot be null");
            Assert.AreEqual(0, args.Count, "Wrong number of input arguments");
        }

        #endregion Description

        #region Execution

        [TestMethod]
        [Description("Processor can be executes through the non-generic base type API")]
        public void Processor_Executes_Via_Base()
        {
            Processor mock = new MockProcessor1();
            mock.Initialize();
            ProcessorResult executionResult = mock.Execute(new object[] { 5 });
            Assert.IsNotNull(executionResult, "ExecutionResult cannot be null");
            object[] result = executionResult.Output;
            Assert.IsNotNull(result, "Inner result cannot be null");
            Assert.AreEqual(1, result.Length, "Expected single output");
            Assert.IsNotNull(result[0], "Expected nonnull output");
            Assert.AreEqual(typeof(string), result[0].GetType(), "Expected string output");
            string resultAsString = (string) result[0];
            Assert.AreEqual("5", resultAsString, "Result incorrect");
        }

        [TestMethod]
        [Description("Processor can be executed directly via strongly-typed API")]
        public void Processor_Executes_Via_Direct_Call()
        {
            MockProcessor1 mock = new MockProcessor1();
            mock.Initialize();
            ProcessorResult<string> executionResult = mock.OnExecute(5);
            Assert.IsNotNull(executionResult, "ExecutionResult cannot be null");
            Assert.AreEqual(5.ToString(), executionResult.Output, "Result incorrect");
        }

        [TestMethod]
        [Description("Processor executes a derived class correctly")]
        public void Processor_Executes_Derived()
        {
            Processor mock = new MockProcessor1Derived();
            mock.Initialize();
            ProcessorResult executionResult = mock.Execute(new object[] { 5 });
            Assert.IsNotNull(executionResult, "ExecutionResult cannot be null");
            object[] result = executionResult.Output;
            Assert.IsNotNull(result, "Inner result cannot be null");
            Assert.AreEqual(1, result.Length, "Expected single output");
            Assert.IsNotNull(result[0], "Expected nonnull output");
            Assert.AreEqual(typeof(string), result[0].GetType(), "Expected string output");
            string resultAsString = (string)result[0];
            Assert.AreEqual("Derived5", resultAsString, "Result incorrect");
        }

        [TestMethod]
        [Description("Processor with nullable inputs functions correctly")]
        public void GenericProcessor_Nullables()
        {
            AssertNullableProcessor<int>(-1, 0, 1, int.MinValue, int.MaxValue);
            AssertNullableProcessor<uint>(0, 1, uint.MinValue, uint.MaxValue);

            AssertNullableProcessor<short>(-1, 0, 1, short.MinValue, short.MaxValue);
            AssertNullableProcessor<ushort>(0, 1, ushort.MinValue, ushort.MaxValue);

            AssertNullableProcessor<long>(-1, 0, 1, long.MinValue, long.MaxValue);
            AssertNullableProcessor<ulong>(0, 1, ulong.MinValue, ulong.MaxValue);

            AssertNullableProcessor<byte>(0, 1, byte.MinValue, byte.MaxValue);
            AssertNullableProcessor<sbyte>(-1, 0, 1, sbyte.MinValue, sbyte.MaxValue);

            AssertNullableProcessor<bool>(true, false);
            AssertNullableProcessor<char>('a', char.MinValue, char.MaxValue);
            AssertNullableProcessor<float>(-1.0f, 0.0f, 1.0f, float.MinValue, float.MaxValue);
            AssertNullableProcessor<double>(-1.0, 0.0, 1.0, double.MinValue, double.MaxValue);
            AssertNullableProcessor<decimal>(-1M, 0M, 1M, decimal.MinValue, decimal.MaxValue);

            AssertNullableProcessor<DateTime>(DateTime.Now, DateTime.UtcNow);
            AssertNullableProcessor<Guid>(Guid.NewGuid());
            AssertNullableProcessor<TimeSpan>(TimeSpan.MinValue, TimeSpan.MaxValue); 
        }

        [TestMethod]
        [Description("Processor with value type inputs functions correctly")]
        public void GenericProcessor_ValueTypes()
        {
            AssertValueTypeProcessor<int>(-1, 0, 1, int.MinValue, int.MaxValue);
            AssertValueTypeProcessor<uint>(0, 1, uint.MinValue, uint.MaxValue);

            AssertValueTypeProcessor<short>(-1, 0, 1, short.MinValue, short.MaxValue);
            AssertValueTypeProcessor<ushort>(0, 1, ushort.MinValue, ushort.MaxValue);

            AssertValueTypeProcessor<long>(-1, 0, 1, long.MinValue, long.MaxValue);
            AssertValueTypeProcessor<ulong>(0, 1, ulong.MinValue, ulong.MaxValue);

            AssertValueTypeProcessor<byte>(0, 1, byte.MinValue, byte.MaxValue);
            AssertValueTypeProcessor<sbyte>(-1, 0, 1, sbyte.MinValue, sbyte.MaxValue);

            AssertValueTypeProcessor<bool>(true, false);
            AssertValueTypeProcessor<char>('a', char.MinValue, char.MaxValue);
            AssertValueTypeProcessor<float>(-1.0f, 0.0f, 1.0f, float.MinValue, float.MaxValue);
            AssertValueTypeProcessor<double>(-1.0, 0.0, 1.0, double.MinValue, double.MaxValue);
            AssertValueTypeProcessor<decimal>(-1M, 0M, 1M, decimal.MinValue, decimal.MaxValue);

            AssertValueTypeProcessor<DateTime>(DateTime.Now, DateTime.UtcNow);
            AssertValueTypeProcessor<Guid>(Guid.NewGuid());
            AssertValueTypeProcessor<TimeSpan>(TimeSpan.MinValue, TimeSpan.MaxValue);
        }


        #endregion Execution

        #region local mocks

        public class MockGeneric2 : Processor<int, string>
        {
            public override ProcessorResult<string> OnExecute(int input)
            {
                return null;
            }
        }

        public class MockGeneric3 : Processor<int, uint, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2)
            {
                return null;
            }
        }

        public class MockGeneric4 : Processor<int, uint, short, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3)
            {
                return null;
            }
        }

        public class MockGeneric5 : Processor<int, uint, short, ushort, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4)
            {
                return null;
            }
        }

        public class MockGeneric6 : Processor<int, uint, short, ushort, long, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5)
            {
                return null;
            }
        }

        public class MockGeneric7 : Processor<int, uint, short, ushort, long, double, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6)
            {
                return null;
            }
        }

        public class MockGeneric8 : Processor<int, uint, short, ushort, long, double, float, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7)
            {
                return null;
            }
        }

        public class MockGeneric9 : Processor<int, uint, short, ushort, long, double, float, DateTime, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8)
            {
                return null;
            }
        }

        public class MockGeneric10 : Processor<int, uint, short, ushort, long, double, float, DateTime, Uri, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8, Uri input9)
            {
                return null;
            }
        }

        public class MockGeneric11 : Processor<int, uint, short, ushort, long, double, float, DateTime, Uri, string, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8, Uri input9, string input10)
            {
                return null;
            }
        }

        public class MockGeneric12 : Processor<int, uint, short, ushort, long, double, float, DateTime, Uri, string, int, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8, Uri input9, string input10, int input11)
            {
                return null;
            }
        }

        public class MockGeneric13 : Processor<int, uint, short, ushort, long, double, float, DateTime, Uri, string, int, uint, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8, Uri input9, string input10, int input11, uint input12)
            {
                return null;
            }
        }

        public class MockGeneric14 : Processor<int, uint, short, ushort, long, double, float, DateTime, Uri, string, int, uint, short, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8, Uri input9, string input10, int input11, uint input12, short input13)
            {
                return null;
            }
        }

        public class MockGeneric15 : Processor<int, uint, short, ushort, long, double, float, DateTime, Uri, string, int, uint, short, ushort, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8, Uri input9, string input10, int input11, uint input12, short input13, ushort input14)
            {
                return null;
            }
        }

        public class MockGeneric16 : Processor<int, uint, short, ushort, long, double, float, DateTime, Uri, string, int, uint, short, ushort, long, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8, Uri input9, string input10, int input11, uint input12, short input13, ushort input14, long input15)
            {
                return null;
            }
        }

        public class MockGeneric17 : Processor<int, uint, short, ushort, long, double, float, DateTime, Uri, string, int, uint, short, ushort, long, double, string>
        {
            public override ProcessorResult<string> OnExecute(int input1, uint input2, short input3, ushort input4, long input5, double input6, float input7, DateTime input8, Uri input9, string input10, int input11, uint input12, short input13, ushort input14, long input15, double input16)
            {
                return null;
            }
        }

        public class MockNonGenericProcessor : Processor
        {
            protected override sealed ProcessorResult OnExecute(object[] input)
            {
                return null;
            }

            protected override IEnumerable<ProcessorArgument> OnGetInArguments()
            {
                return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
            }

            protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
            {
                return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
            }
 
        }
        // This mock works like our normal generic processors, but
        // is concrete, allowing us to test different permutations of
        // finding OnExecute via Reflection
        public class MockGenericProcessor<T, TOutput> : Processor
        {
            protected override sealed ProcessorResult OnExecute(object[] input)
            {
                return null;
            }

            protected override IEnumerable<ProcessorArgument> OnGetInArguments()
            {
                return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildInputArgumentCollection();
            }

            protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
            {
                return new ReflectionProcessorArgumentBuilder(this.GetType()).BuildOutputArgumentCollection();
            }
        }

        // Invalid -- class has no OnExecute
        public class ProcessorWithNoExecute : MockGenericProcessor<int, string>
        {
        }

        // Invalid -- class has OnExecute with wrong return type
        public class ProcessorWithWrongExecuteReturn : MockGenericProcessor<int, string>
        {
            public double OnExecute(int i) { return 0.0; }
        }

        // Invalid -- class has OnExecute with wrong return type generic
        public class ProcessorWithWrongExecuteReturnGeneric : MockGenericProcessor<int, string>
        {
            public ProcessorResult<double> OnExecute(int i) { return null; }
        }

        // Invalid -- class has OnExecute with wrong parameter type
        public class ProcessorWithWrongExecuteParameterType : MockGenericProcessor<int, string>
        {
            public ProcessorResult<string> OnExecute(double d) { return null; }
        }

        // Invalid -- class has OnExecute with wrong parameter count
        public class ProcessorWithWrongExecuteParameterCount : MockGenericProcessor<int, string>
        {
            public ProcessorResult<string> OnExecute(int i, double d) { return null; }
        }

        // Valid -- class has valid OnExecute in the midst of other illegal combinations
        public class ProcessorWithValidAndInvalidExecute : MockGenericProcessor<int, string>
        {
            public ProcessorResult<string> OnExecute() { return null; }
            public ProcessorResult<string> OnExecute(int i, double d) { return null; }
            public ProcessorResult<string> OnExecute(int intValue) { return null; }
        }
        #endregion local mocks

        #region test helpers
        private void AssertNullableProcessor<T>(params T[] values) where T : struct
        {
            MockNullableProcessor<T> processor = new MockNullableProcessor<T>();
            processor.Initialize();

            ProcessorResult<string> result = processor.CallExecute(new object[] { null }) as ProcessorResult<string>;
            Assert.IsNotNull(result, "Expected ProcessorResult<string> for nullable processor of " + typeof(T).Name);
            Assert.AreEqual(MockNullableProcessor<T>.NOVALUE, result.Output, "Expected no value for null for nullable processor of " + typeof(T).Name);

            foreach (T value in values)
            {
                result = processor.CallExecute(new object[] { value }) as ProcessorResult<string>;
                Assert.IsNotNull(result, "Expected ProcessorResult<string> for nullable processor of " + typeof(T).Name);
                Assert.AreEqual(value.ToString(), result.Output, "Wrong value for nullable processor of " + typeof(T).Name);
            }

            // Attempting to execute with a non-compatible type should detect and cause error
            ProcessorResult errorResult = null;
            processor.OnErrorCalled = r => errorResult = r;
            ProcessorResult theResult = processor.CallExecute(new object[] { "incorrect type" });
            Assert.AreEqual(ProcessorStatus.Error, theResult.Status, "Expected error status for nullable processor of " + typeof(T).Name);
            Assert.IsNotNull(errorResult, "Expected OnError to be called for nullable processor of " + typeof(T).Name);
        }

        private void AssertValueTypeProcessor<T>(params T[] values) where T : struct
        {
            MockValueTypeProcessor<T> processor = new MockValueTypeProcessor<T>();
            processor.Initialize();
            ProcessorResult<string> result;

            foreach (T value in values)
            {
                result = processor.CallExecute(new object[] { value }) as ProcessorResult<string>;
                Assert.IsNotNull(result, "Expected ProcessorResult<string> for value type processor of " + typeof(T).Name);
                Assert.AreEqual(value.ToString(), result.Output, "Wrong value for valueType processor of " + typeof(T).Name);
            }

            // Attempting to execute with a null should detect and cause error

            ProcessorResult theResult = processor.CallExecute(new object[] { null });
            Assert.AreEqual(ProcessorStatus.Ok, theResult.Status, "Expected OK status for value type processor of " + typeof(T).Name);
            Assert.AreEqual(default(T).ToString(), theResult.Output[0], "Expected default(T) for value type processor of " + typeof(T).Name);

            // Attempting to execute with a non-compatible type should detect and cause error
            ProcessorResult errorResult = null;
            processor.OnErrorCalled = r => errorResult = r;
            theResult = processor.CallExecute(new object[] { "incorrect type" });
            Assert.AreEqual(ProcessorStatus.Error, theResult.Status, "Expected error status for value type processor of " + typeof(T).Name);
            Assert.IsNotNull(errorResult, "Expected OnError to be called for value type processor of " + typeof(T).Name);
        }
        #endregion test helpers
    }
}
