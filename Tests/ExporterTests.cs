﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using libEDSsharp;

namespace Tests
{
    [TestClass]
    public class ExporterTests : CanOpenNodeExporter
    {
        [TestMethod]
        public void Test_cname_conversion()
        {

            if (make_cname("axle 0 wheel right controlword") != "axle0WheelRightControlword")
                throw (new Exception("make_cname Conversion error"));

            if (make_cname("mapped object 4") != "mappedObject4")
                throw (new Exception("make_cname Conversion error"));

            if (make_cname("COB ID used by RPDO") != "COB_IDUsedByRPDO")
                throw (new Exception("make_cname Conversion error"));
        }

        [TestMethod]
        public void Test_record_objects()
        {

            ODentry od = new ODentry();
            od.objecttype = ObjectType.REC;
            od.parameter_name = "Test Record";
            od.accesstype = EDSsharp.AccessType.ro;
            od.index  = 0x2000;

            ODentry subod = new ODentry("Test String 1", 0x01, 0x01, DataType.VISIBLE_STRING, "abcdefg", EDSsharp.AccessType.rw, PDOMappingType.optional, od);

            string test = export_one_record_type(subod, "");

            if (test != "           {(void*)&CO_OD_RAM.testRecord.testString1, 0xbe, 0x7 },\n") 
                throw (new Exception("export_one_record_type() error test 1"));

            subod = new ODentry("Test String 2", 0x01, 0x01, DataType.VISIBLE_STRING, new string('*', 255), EDSsharp.AccessType.ro, PDOMappingType.optional, od);
            test = export_one_record_type(subod, "");

            if (test != "           {(void*)&CO_OD_RAM.testRecord.testString2, 0xa6, 0xff },\n")
                throw (new Exception("export_one_record_type() error test 2"));

        }

        [TestMethod]
        public void TestArrays()
        {

            ODentry od = new ODentry();
            od.objecttype = ObjectType.ARRAY;
            od.datatype = DataType.VISIBLE_STRING;
            od.parameter_name = "Test Array";
            od.accesstype = EDSsharp.AccessType.ro;
            od.index = 0x2000;

            eds = new EDSsharp();
            eds.ods = new System.Collections.Generic.SortedDictionary<ushort, ODentry>();

            eds.ods.Add(0x2000,od);

            prewalkArrays();
            od.subobjects.Add(0x00, new ODentry("No Entries", 0x00, DataType.UNSIGNED8, "4", EDSsharp.AccessType.ro, PDOMappingType.no));
            od.subobjects.Add(0x01, new ODentry("LINE1", 0x01, DataType.VISIBLE_STRING,new string('*',1),EDSsharp.AccessType.ro,PDOMappingType.optional));
            od.subobjects.Add(0x02, new ODentry("LINE1", 0x02, DataType.VISIBLE_STRING, new string('*', 10), EDSsharp.AccessType.ro, PDOMappingType.optional));
            od.subobjects.Add(0x03, new ODentry("LINE1", 0x03, DataType.VISIBLE_STRING, new string('*', 16), EDSsharp.AccessType.ro, PDOMappingType.optional));
            od.subobjects.Add(0x04, new ODentry("LINE1", 0x04, DataType.VISIBLE_STRING, new string('*', 32), EDSsharp.AccessType.ro, PDOMappingType.optional));

            string test = print_h_entry(od);

            if(test != "/*2000      */ VISIBLE_STRING  testArray[32][4];\n")
                throw (new Exception("TestArrays() test 1 failed"));


        }

    }
}
