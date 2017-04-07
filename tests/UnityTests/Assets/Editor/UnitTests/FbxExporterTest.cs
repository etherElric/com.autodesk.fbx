﻿using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System.IO;

namespace UnitTests
{

    public class FbxExporterTest
    {
        FbxManager m_fbxManager;
        FbxExporter m_exporter;

        string m_testFolderPrefix = "to_delete_";
        string m_testFolder;

        private string GetRandomDirectory()
        {
            string randomDir = Path.Combine(Path.GetTempPath(), m_testFolderPrefix);

            string temp;
            do {
                // check that the directory does not already exist
                temp = randomDir + Path.GetRandomFileName ();
            } while(Directory.Exists (temp));

            return temp;
        }

        private string GetRandomFilename(string path, bool fbxExtension = true)
        {
            string temp;
            do {
                // check that the directory does not already exist
                temp = Path.Combine (path, Path.GetRandomFileName ());

                if(fbxExtension){
                    temp = Path.ChangeExtension(temp, ".fbx");
                }

            } while(File.Exists (temp));

            return temp;
        }

        [SetUp]
        public void InitBeforeTest()
        {
            m_fbxManager = FbxManager.Create ();
            m_exporter = FbxExporter.Create (m_fbxManager, "exporter");

            Assert.IsNotNull (m_exporter);

            // make sure directory does not already exist
            //Directory.Delete(m_testFolder, true);

            var testDirectories = Directory.GetDirectories(Path.GetTempPath(), m_testFolderPrefix + "*");

            foreach (var directory in testDirectories)
            {
                Directory.Delete(directory, true);
            }

            m_testFolder = GetRandomDirectory ();
            Directory.CreateDirectory (m_testFolder);
        }

        [TearDown]
        public void CleanupAfterTest()
        {
            try{
                m_exporter.Destroy();
                m_fbxManager.Destroy();
            }
            catch(System.ArgumentNullException){
                // already destroyed in test
            }

            // delete all files that were created
            Directory.Delete(m_testFolder, true);
        }

        [Test]
        public void TestExportEmptyFbxDocument ()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, m_fbxManager.GetIOSettings());

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }


        [Test]
        public void TestExportNull ()
        {
            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, m_fbxManager.GetIOSettings());

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (null);

            Assert.IsFalse (status);

            // FbxSdk creates an empty file even though the export status was false
            Assert.IsTrue (File.Exists (filename));
        }
            
        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroy ()
        {
            m_exporter.Destroy ();
            m_exporter.GetName ();
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyManager ()
        {
            m_fbxManager.Destroy ();
            m_exporter.GetName ();
        }

        [Test]
        public void TestInitializeInvalidFilenameOnly()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder, false);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);

            // FbxSdk doesn't create a file in this situation
            Assert.IsFalse (File.Exists (filename));
        }

        [Test]
        public void TestInitializeValidFilenameOnly()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeInvalidFileFormat()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, int.MinValue);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            // looks like anything less than 0 is treated the same as -1
            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeInvalidFileFormat2()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, int.MaxValue);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsFalse (status);
            Assert.IsFalse (File.Exists (filename));
        }

        [Test]
        public void TestInitializeValidFileFormat()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, 1);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeNullIOSettings()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, null);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        [Ignore("Crashes Unity when passed null FbxManager to FbxIOSettings")]
        public void TestInitializeInvalidIOSettings()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, FbxIOSettings.Create(null, ""));

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }
    }
}