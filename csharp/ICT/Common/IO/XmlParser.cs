//
// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       timop
//
// Copyright 2004-2010 by OM International
//
// This file is part of OpenPetra.org.
//
// OpenPetra.org is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// OpenPetra.org is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with OpenPetra.org.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Xml;
using System.Xml.Schema;
using System.Collections;
using System.IO;

namespace Ict.Common.IO
{
    /// <summary>
    /// This class represents one single entity.
    /// It is normally used as a super class to a specific object type
    ///
    /// </summary>
    public class TXMLElement
    {
        /// <summary>
        /// if the element is one of many in the group, here the order is saved
        ///
        /// </summary>
        public int order;

        /// <summary>
        /// initialises the element with the given order
        /// </summary>
        /// <param name="order">the index of the element in the group it belongs to
        /// </param>
        /// <returns>void</returns>
        public TXMLElement(int order) : base()
        {
            this.order = order;
        }
    }

    /// <summary>
    /// This class represents a group of entities.
    ///
    /// </summary>
    public class TXMLGroup
    {
        /// <summary>
        /// list of the entities in this group
        /// </summary>
        public ArrayList List;

        /// <summary>
        /// identifier for the group
        /// </summary>
        public int Id;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id"></param>
        public TXMLGroup(int id) : base()
        {
            List = new ArrayList();
            this.Id = id;
        }
    }

    /// <summary>
    /// This class provides methods for parsing an XML document and assign the contents
    /// to a representation in memory.
    ///
    /// The TXMLParser class provides an easy approach to parse elements
    /// and groups of elements of an XML document.
    /// It provides functions for easily navigate through an XML document
    /// and to retrieve attributes etc.
    /// It makes use of the .Net XML parser library.
    /// </summary>
    public class TXMLParser
    {
        /// <summary>
        /// the XmlDocument that is currently parsed
        /// </summary>
        protected XmlDocument myDoc;
        static string XMLFilePathForDTD = String.Empty;

        /// <summary>
        /// this fixes the problem that we have the filename of the DTD with a relative path name in the XML file
        /// this only works for situations with the dtd file in the same directory as the xml file
        /// </summary>
        private class MyUrlResolver : XmlUrlResolver
        {
            string FXmlFilePath = String.Empty;

            /// <summary>
            /// pass the path of the xml file to build the proper path for the dtd file
            /// </summary>
            /// <param name="AXmlFilePath"></param>
            public MyUrlResolver(string AXmlFilePath)
            {
                FXmlFilePath = Path.GetFullPath(AXmlFilePath);
            }

            /// <summary>
            /// overload this method to get the dtd from the same directory as the xml file
            /// </summary>
            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                if (!File.Exists(absoluteUri.AbsolutePath))
                {
                    string dtdFilename = FXmlFilePath + Path.DirectorySeparatorChar + Path.GetFileName(absoluteUri.AbsolutePath);

                    if (!File.Exists(dtdFilename))
                    {
                        // we have some reports in a directory below where the reports.dtd is
                        dtdFilename = FXmlFilePath + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + Path.GetFileName(
                            absoluteUri.AbsolutePath);
                    }

                    return File.Open(dtdFilename, FileMode.Open, FileAccess.Read);
                }

                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="withValidation"></param>
        public TXMLParser(string filename, Boolean withValidation)
        {
            ValidationEventHandler eventHandler;

            if (!System.IO.File.Exists(filename))
            {
                throw new Exception("file " + filename + " could not be found.");
            }

            eventHandler = new ValidationEventHandler(ValidationCallback);
            XmlReader reader = null;
            StreamReader sr = null;

            try
            {
                // TODO there seems to be problems finding the dtd file on Mono; so no validation there for the moment
                // also see http://sourceforge.net/apps/mantisbt/openpetraorg/view.php?id=52
                if (Ict.Common.Utilities.DetermineExecutingCLR() == TExecutingCLREnum.eclrMono)
                {
                    withValidation = false;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = false;
                settings.ProhibitDtd = false;
                settings.XmlResolver = new MyUrlResolver(Path.GetDirectoryName(filename));
                settings.ValidationType = withValidation ? ValidationType.DTD : ValidationType.None;
                settings.ValidationEventHandler += new ValidationEventHandler(eventHandler);

                sr = new StreamReader(filename);
                reader = XmlReader.Create(sr, settings);

                myDoc = new XmlDocument();
                myDoc.Load(reader);
            }
            catch (Exception e)
            {
                if (reader != null)
                {
                    sr.Close();
                    reader.Close();
                }

                // throw the exception again,
                // so that the error message of the validation and wellformedness checks
                // can be displayed to the user
                throw e;
            }
            sr.Close();
            reader.Close();
        }

        /// <summary>
        /// constructor; opens the file with validation
        /// </summary>
        /// <param name="filename"></param>
        public TXMLParser(string filename) : this(filename, true)
        {
        }

        /// <summary>
        /// can be used for document that is parsed from YML
        /// </summary>
        /// <param name="ADoc">the document that should be parsed</param>
        public TXMLParser(XmlDocument ADoc)
        {
            myDoc = ADoc;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="AReuseParser"></param>
        public TXMLParser(TXMLParser AReuseParser) : base()
        {
            this.myDoc = AReuseParser.myDoc;
        }

        /// <summary>
        /// this method is called if validation fails
        /// </summary>
        /// <param name="sender">who is sending this</param>
        /// <param name="args">information about the validation error</param>
        void ValidationCallback(object sender, ValidationEventArgs args)
        {
            throw new Exception("Error: Validation error loading xml file: " + Environment.NewLine + args.Message);
        }

        /// <summary>
        /// retrieves the document, in case the document should be parsed not by a derived class
        ///
        /// </summary>
        /// <returns>void</returns>
        public XmlDocument GetDocument()
        {
            return myDoc;
        }

        /// <summary>
        /// print an xml document to string;
        /// this can be necessary for transmitting between server and client, since XmlDocument is not serializable
        /// </summary>
        /// <param name="ADoc"></param>
        /// <returns></returns>
        public static string XmlToString(XmlDocument ADoc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);

            ADoc.WriteTo(xw);
            return sw.ToString();
        }

        /// <summary>
        /// print an xml document to string with indentation
        /// </summary>
        /// <param name="ADoc"></param>
        /// <returns></returns>
        public static string XmlToStringIndented(XmlDocument ADoc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);

            xw.Formatting = Formatting.Indented;

            ADoc.WriteTo(xw);
            return sw.ToString();
        }

        /// used by XmlToString
        private static void MoveElementNameToAttribute(XmlNode AOldNode, XmlNode ANewNode)
        {
            // create name attribute
            if ((AOldNode.Name != TYml2Xml.XMLELEMENT) && (AOldNode.Name != TYml2Xml.ROOTNODEINTERNAL))
            {
                XmlAttribute attr = ANewNode.OwnerDocument.CreateAttribute("name");
                attr.Value = AOldNode.Name;
                ANewNode.Attributes.Append(attr);
            }

            // copy all attributes, apart from depth == 0
            foreach (XmlAttribute attrOld in AOldNode.Attributes)
            {
                if (!((attrOld.Name == "depth") && (attrOld.Value == "0")))
                {
                    XmlAttribute attrNew = ANewNode.OwnerDocument.CreateAttribute(attrOld.Name);
                    attrNew.Value = attrOld.Value;
                    ANewNode.Attributes.Append(attrNew);
                }
            }

            // create node for each child, and call recursively
            foreach (XmlNode child in AOldNode.ChildNodes)
            {
                XmlElement newChildNode = ANewNode.OwnerDocument.CreateElement(TYml2Xml.XMLELEMENT);
                ANewNode.AppendChild(newChildNode);

                MoveElementNameToAttribute(child, newChildNode);
            }
        }

        /// <summary>
        /// this is necessary to be able to store XML files that can be parsed by DTD/Schema mechanisms;
        /// for YML, we have the name of an element not as an attribute, but the element name itself;
        /// this would never work for validating xml; therefore all Elements are called XmlElement, with attribute name
        /// </summary>
        /// <param name="ADoc"></param>
        /// <param name="AMoveElementNamesToAttribute"></param>
        /// <returns></returns>
        public static string XmlToString(XmlDocument ADoc, bool AMoveElementNamesToAttribute)
        {
            XmlDocument tempDoc = TYml2Xml.CreateXmlDocument();

            MoveElementNameToAttribute(ADoc.DocumentElement, tempDoc.DocumentElement);

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            tempDoc.WriteTo(xw);
            return sw.ToString();
        }

        /// <summary>
        /// test the given node, if it is a comment or is empty; if it is empty, try the next nodes.
        /// </summary>
        /// <param name="cur2">the current node</param>
        /// <returns>the current or the first next node with actual content
        /// </returns>
        public static XmlNode NextNotBlank(XmlNode cur2)
        {
            XmlNode cur;

            cur = cur2;

            while (true)
            {
                if (cur == null)
                {
                    return cur;
                }

                if ((cur.NodeType == System.Xml.XmlNodeType.Text) && (cur.ToString().Length == 0))
                {
                    cur = cur.NextSibling;
                    continue;
                }

                if (cur.Name == "comment")
                {
                    cur = cur.NextSibling;
                    continue;
                }

                if (cur.Name == "#comment")
                {
                    cur = cur.NextSibling;
                    continue;
                }

                return cur;
            }
        }

        /// <summary>
        /// get the next code that is a proper entity (i.e. no comment, not empty)
        /// </summary>
        /// <param name="cur">the current node</param>
        /// <returns>the next node with actual content
        /// </returns>
        public static XmlNode GetNextEntity(XmlNode cur)
        {
            XmlNode ReturnValue;

            if (cur == null)
            {
                ReturnValue = null;
            }
            else
            {
                ReturnValue = NextNotBlank(cur.NextSibling);
            }

            return ReturnValue;
        }

        /// <summary>
        /// return the child with the given name, if it exists; otherwise return null
        /// </summary>
        /// <returns>void</returns>
        public static XmlNode GetChild(XmlNode cur, string name)
        {
            for (Int32 counter = 0; counter < cur.ChildNodes.Count; counter++)
            {
                if (cur.ChildNodes[counter].Name == name)
                {
                    return cur.ChildNodes[counter];
                }
            }

            return null;
        }

        /// <summary>
        /// find a node somewhere in the xml document by its name
        /// </summary>
        /// <param name="AParentNode"></param>
        /// <param name="ANodeNameToSearch"></param>
        /// <returns></returns>
        public static XmlNode FindNodeRecursive(XmlNode AParentNode, string ANodeNameToSearch)
        {
            XmlNode ResultNode = GetChild(AParentNode, ANodeNameToSearch);

            if (ResultNode == null)
            {
                foreach (XmlNode childNode in AParentNode.ChildNodes)
                {
                    ResultNode = FindNodeRecursive(childNode, ANodeNameToSearch);

                    if (ResultNode != null)
                    {
                        return ResultNode;
                    }
                }
            }

            return ResultNode;
        }

        /// <summary>
        /// add or set an attribute value
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="AAttribName"></param>
        /// <param name="AAttribValue"></param>
        public static void SetAttribute(XmlNode cur, string AAttribName, string AAttribValue)
        {
            if (cur.Attributes[AAttribName] != null)
            {
                cur.Attributes[AAttribName].Value = AAttribValue;
            }
            else
            {
                XmlAttribute newAttrib = cur.OwnerDocument.CreateAttribute(AAttribName);
                newAttrib.Value = AAttribValue;
                cur.Attributes.Append(newAttrib);
            }
        }

        /// <summary>
        /// rename an existing attribute
        /// </summary>
        /// <param name="cur"></param>
        /// <param name="AAttribOldName"></param>
        /// <param name="AAttribNewName"></param>
        public static void RenameAttribute(XmlNode cur, string AAttribOldName, string AAttribNewName)
        {
            if (cur.Attributes[AAttribOldName] != null)
            {
                SetAttribute(cur, AAttribNewName, cur.Attributes[AAttribOldName].Value);
                cur.Attributes.Remove(cur.Attributes[AAttribOldName]);
            }
        }

        /// <summary>
        /// retrieve the value of an attribute. Does prevent unnecessary exceptions, if the attribute is not existing
        /// </summary>
        /// <param name="cur">the current node</param>
        /// <param name="attrib">the name of the attribute</param>
        /// <returns>the value of the attribute, or an empty string if it is not existing
        /// </returns>
        public static string GetAttribute(XmlNode cur, string attrib)
        {
            string ReturnValue;
            XmlNode node;

            ReturnValue = "";
            node = cur.Attributes.GetNamedItem(attrib);

            if (node != null)
            {
                ReturnValue = node.Value;
                ReturnValue = ReturnValue.Replace(" <br/>", Environment.NewLine);
                ReturnValue = ReturnValue.Replace("<br/>", Environment.NewLine);
            }

            return ReturnValue;
        }

        /// <summary>
        /// retrieve whether the node has an attribute with the given name or not
        /// </summary>
        /// <param name="cur">the current node</param>
        /// <param name="attrib">the name of the attribute</param>
        /// <returns>true if the attribute is existing
        /// </returns>
        public static Boolean HasAttribute(XmlNode cur, string attrib)
        {
            if (cur.Attributes == null)
            {
                return false;
            }

            XmlNode node = cur.Attributes.GetNamedItem(attrib);

            if (node != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// retrieve the int value of an attribute. Does prevent unnecessary exceptions, if the attribute is not existing
        /// </summary>
        /// <param name="cur">the current node</param>
        /// <param name="attrib">the name of the attribute</param>
        /// <returns>the value of the attribute, or -1 if it is not existing
        /// </returns>
        public static Int32 GetIntAttribute(XmlNode cur, string attrib)
        {
            Int32 ReturnValue;
            XmlNode node;

            ReturnValue = -1;
            node = cur.Attributes.GetNamedItem(attrib);
            try
            {
                if (node != null)
                {
                    ReturnValue = Convert.ToInt32(node.Value);
                }
            }
            catch (Exception)
            {
                ReturnValue = -1;
            }
            return ReturnValue;
        }

        /// <summary>
        /// retrieve the boolean value of an attribute. Understands all sorts of notation (yes|no, true|false, 0|1)
        /// </summary>
        /// <param name="cur">the current node</param>
        /// <param name="attrib">the name of the attribute</param>
        /// <param name="defaultvalue">the default value if the attribute is not existing; optional: false</param>
        /// <returns>the value of the attribute, or the default value if it is not existing
        /// </returns>
        public static bool GetBoolAttribute(XmlNode cur, string attrib, bool defaultvalue)
        {
            bool ReturnValue;
            string value;
            XmlNode node;

            ReturnValue = defaultvalue;

            node = cur.Attributes.GetNamedItem(attrib);

            if (node != null)
            {
                value = node.Value.ToLower().Trim();

                if ((value == "yes") || (value == "true") || (value == "1"))
                {
                    ReturnValue = true;
                }
                else if ((value == "no") || (value == "false") || (value == "0"))
                {
                    ReturnValue = false;
                }
            }

            return ReturnValue;
        }

        /// <summary>
        /// overload for GetBoolAttribute, assuming default value is false
        /// </summary>
        /// <param name="cur">the current node</param>
        /// <param name="attrib">the name of the attribute</param>
        /// <returns>the value of the attribute, or false if it is not existing
        /// </returns>
        public static bool GetBoolAttribute(XmlNode cur, string attrib)
        {
            return GetBoolAttribute(cur, attrib, false);
        }

        /// <summary>
        /// retrieve the double value of an attribute. Does prevent unnecessary exceptions, if the attribute is not existing
        /// </summary>
        /// <param name="cur">the current node</param>
        /// <param name="attrib">the name of the attribute</param>
        /// <returns>the value of the attribute, or -1 if it is not existing
        /// </returns>
        public static double GetDoubleAttribute(XmlNode cur, string attrib)
        {
            double ReturnValue;
            XmlNode node;

            ReturnValue = -1;
            node = cur.Attributes.GetNamedItem(attrib);
            try
            {
                if (node != null)
                {
                    ReturnValue = Convert.ToDouble(node.Value);
                }
            }
            catch (Exception)
            {
                ReturnValue = -1;
            }
            return ReturnValue;
        }
    }

    /// <summary>
    /// a parser for a group of entities
    /// </summary>
    public class TXMLGroupParser : TXMLParser
    {
        /// <summary>
        /// constructor; opens an xml document, with validation
        /// </summary>
        /// <param name="filename">name of the xml document</param>
        public TXMLGroupParser(string filename) : base(filename)
        {
        }

        /// <summary>
        /// constructor; opens an xml document
        /// </summary>
        /// <param name="filename">name of the xml document</param>
        /// <param name="withValidation">should the document be validated</param>
        public TXMLGroupParser(string filename, Boolean withValidation) : base(filename, withValidation)
        {
        }

        /// <summary>
        /// This method needs to be implemented by a derived class,
        /// because it needs to know how to parse all the entities in the document.
        /// It is a template for reading a group of entities.
        /// </summary>
        /// <param name="cur2">The current node in the document that should be parsed</param>
        /// <param name="groupId">The id of the previous group object of this type</param>
        /// <param name="entity">The name of the elements that make up the group</param>
        /// <param name="newcur">After parsing, the then current node is returned in this parameter</param>
        /// <returns>The group of elements, or nil
        /// </returns>
        public virtual TXMLGroup ParseGroup(XmlNode cur2, ref int groupId, string entity, ref XmlNode newcur)
        {
            return null;
        }

        /// <summary>
        /// This method needs to be implemented by a derived class,
        /// because it needs to know how to parse all the entities in the document.
        /// It is a template for reading one or more entities of the same type.
        /// </summary>
        /// <param name="cur">The current node</param>
        /// <param name="groupId">The id of the current group that the elements will belong to</param>
        /// <param name="entity">The name of the entity that should be read</param>
        /// <returns>the last of the read entities
        /// </returns>
        public virtual TXMLElement Parse(XmlNode cur, ref int groupId, string entity)
        {
            return null;
        }

        /// <summary>
        /// If the current node holds a entity of type groupentity,
        /// then the contained elements of type entity are parsed.
        /// </summary>
        /// <param name="groupentity">The name of the entity that holds the group of elements</param>
        /// <param name="cur">The current node</param>
        /// <param name="groupId">The current id for the group.
        /// This number will be increased and then used to create the new group.</param>
        /// <param name="entity">The name of the entitiy that represents the elements</param>
        /// <returns>The group of elements, or nil if node does not point to that group entity or the group entity is empty.
        /// </returns>
        TXMLGroup Parse(string groupentity, ref XmlNode cur, ref int groupId, string entity)
        {
            TXMLGroup ReturnValue;

            ReturnValue = null;

            if (cur.Name == groupentity)
            {
                cur = NextNotBlank(cur);
                ReturnValue = ParseGroup(cur.FirstChild, ref groupId, entity);
                cur = GetNextEntity(cur);
            }

            return ReturnValue;
        }

        /// <summary>
        /// a wrapper for the other parseGroup method,
        /// in case after the parsing the current node does not need to be known
        /// </summary>
        /// <param name="cur2">The current node in the document that should be parsed</param>
        /// <param name="groupId">The id of the previous group object of this type</param>
        /// <param name="entity">The name of the elements that make up the group</param>
        /// <returns>The group of elements, or nil
        /// </returns>
        TXMLGroup ParseGroup(XmlNode cur2, ref int groupId, string entity)
        {
            XmlNode curTemp = null;

            return ParseGroup(cur2, ref groupId, entity, ref curTemp);
        }
    }
}