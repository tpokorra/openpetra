/* Auto generated with nant generateORM
 * Do not modify this file manually!
 */
{#GPLFILEHEADER}
namespace {#NAMESPACE}
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Odbc;
    using System.Runtime.Serialization;
    using System.Xml;
    using Ict.Common;
    using Ict.Common.Data;

    {#TABLELOOP}
}

{##TYPEDTABLE}

{#TABLE_DESCRIPTION}
[Serializable()]
public class {#TABLENAME}Table : TTypedDataTable
{
    {#DATACOLUMNS}
    
    /// TableId for Ict.Common.Data generic functions
    public static short TableId = {#TABLEID};
    
    private static bool FInitInfoValues = InitInfoValues();
    private static bool InitInfoValues()
    {
        TableInfo.Add(TableId, new TTypedTableInfo(TableId, "{#TABLENAME}", "{#DBTABLENAME}", 
            new TTypedColumnInfo[] { 
                {#COLUMNINFO}
            },
            new int[] { 
                {#COLUMNPRIMARYKEYORDER}
            }));
        return true;
    }

    /// constructor
    public {#TABLENAME}Table() : 
            base("{#TABLENAME}")
    {
    }
    
    /// constructor
    public {#TABLENAME}Table(string ATablename) : 
            base(ATablename)
    {
    }
    
    /// constructor for serialization
    public {#TABLENAME}Table(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
            base(info, context)
    {
    }

    /// create the columns
    protected override void InitClass()
    {
        {#INITCLASSADDCOLUMN}
    }
    
    /// assign columns to properties, set primary key
    public override void InitVars()
    {
        {#INITVARSCOLUMN}
{#IFDEF PRIMARYKEYCOLUMNS}
        this.PrimaryKey = new System.Data.DataColumn[] {
                {#PRIMARYKEYCOLUMNS}};
{#ENDIF PRIMARYKEYCOLUMNS}
    }

    /// Access a typed row by index
    public {#TABLENAME}Row this[int i]
    {
        get
        {
            return (({#TABLENAME}Row)(this.Rows[i]));
        }
    }

    /// create a new typed row
    public {#TABLENAME}Row NewRowTyped(bool AWithDefaultValues)
    {
        {#TABLENAME}Row ret = (({#TABLENAME}Row)(this.NewRow()));
        if ((AWithDefaultValues == true))
        {
            ret.InitValues();
        }
        return ret;
    }
    
    /// create a new typed row, always with default values
    public {#TABLENAME}Row NewRowTyped()
    {
        return this.NewRowTyped(true);
    }
    
    /// new typed row using DataRowBuilder
    protected override System.Data.DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
    {
        return new {#TABLENAME}Row(builder);
    }
    
    /// get typed set of changes
    public {#TABLENAME}Table GetChangesTyped()
    {
        return (({#TABLENAME}Table)(base.GetChangesTypedInternal()));
    }

    /// get an odbc parameter for the given column
    public override OdbcParameter CreateOdbcParameter(Int32 AColumnNr)
    {
        return CreateOdbcParameter(TableId, AColumnNr);
    }

    {#STATICCOLUMNPROPERTIES}

}

{##DATACOLUMN}
{#COLUMN_DESCRIPTION}
public DataColumn Column{#COLUMNNAME};

{##COLUMNINFO}
new TTypedColumnInfo({#COLUMNORDERNUMBER}, "{#COLUMNNAME}", "{#COLUMNDBNAME}", {#COLUMNODBCTYPE}, {#COLUMNLENGTH}, {#COLUMNNOTNULL}){#COLUMNCOMMA}

{##INITCLASSADDCOLUMN}
this.Columns.Add(new System.Data.DataColumn("{#COLUMNDBNAME}", typeof({#COLUMNDOTNETTYPE})));

{##INITVARSCOLUMN}
this.Column{#COLUMNNAME} = this.Columns["{#COLUMNDBNAME}"];

{##STATICCOLUMNPROPERTIES}

/// get the name of the field in the database for this column
public static string Get{#COLUMNNAME}DBName()
{
    return "{#COLUMNDBNAME}";
}

/// get character length for column
public static short Get{#COLUMNNAME}Length()
{
    return {#COLUMNLENGTH};
}

{##TYPEDROW}

{#TABLE_DESCRIPTION}
[Serializable()]
public class {#TABLENAME}Row : System.Data.DataRow
{
    private {#TABLENAME}Table myTable;
    
    /// Constructor
    public {#TABLENAME}Row(System.Data.DataRowBuilder rb) : 
            base(rb)
    {
        this.myTable = (({#TABLENAME}Table)(this.Table));
    }

    {#ROWCOLUMNPROPERTIES}

    /// set default values
    public virtual void InitValues()
    {
        {#ROWSETNULLORDEFAULT}
    }

    {#FUNCTIONSFORNULLVALUES}
}

{##ROWCOLUMNPROPERTY}

{#COLUMN_DESCRIPTION}
public {#COLUMNDOTNETTYPE} {#COLUMNNAME}
{
    get
    {
        object ret;
        ret = this[this.myTable.Column{#COLUMNNAME}.Ordinal];
        if ((ret == System.DBNull.Value))
        {
            throw new System.Data.StrongTypingException("Error: DB null", null);
        }
        else
        {
            return (({#COLUMNDOTNETTYPE})(ret));
        }
    }
    set
    {
        if ((this.IsNull(this.myTable.Column{#COLUMNNAME}) 
                    || ((({#COLUMNDOTNETTYPE})(this[this.myTable.Column{#COLUMNNAME}])) != value)))
        {
            this[this.myTable.Column{#COLUMNNAME}] = value;
        }
    }
}

{##FUNCTIONSFORNULLVALUES}

/// test for NULL value
public bool Is{#COLUMNNAME}Null()
{
    return this.IsNull(this.myTable.Column{#COLUMNNAME});
}

/// assign NULL value
public void Set{#COLUMNNAME}Null()
{
    this.SetNull(this.myTable.Column{#COLUMNNAME});
}
