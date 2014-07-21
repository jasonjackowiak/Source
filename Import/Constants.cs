using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Import
{

  public static class Constants
  {
	#region text file extract constants

	  // Table extract fields positions and lengths - start
    public const int posTableName = 30; // position of the table name in the table extract
    public const int lenTableName = 16; // length of the table name in the table extract
    public const int posFieldName = 46; // position of the field name in the table extract
    public const int lenFieldName = 16; // length of the field name in the table extract
    public const int posFieldType = 62; // position of the field type in the table extract
    public const int lenFieldType = 1; // length of the field type in the table extract
    public const int posFieldSyntax = 63; // position of the field syntax in the table extract
    public const int lenFieldSyntax = 1; // length of the field syntax in the table extract
    public const int posFieldLength = 64; // position of the field length in the table extract
    public const int lenFieldLength = 6; // length of the field length in the table extract
    public const int posFieldDecimal = 70; // position of the field number of decimal places in the table extract
    public const int lenFieldDecimal = 6; // length of the field field number of decimal places in the table extract
    public const int posFieldNumber = 76; // position of the field sequential number in the table extract
    public const int lenFieldNumber = 6; // length of the field sequential number in the table extract
    public const int posKeyType = 82; // position of the field key type in the table extract
    public const int lenKeyType = 1; // length of the field key type in the table extract
    public const int posTableUnit = 83; // position of the table unit type in the table extract
    public const int lenTableUnit = 8; // length of the table unit type in the table extract
    public const int posTableType = 91; // position of the table table type in the table extract
    public const int lenTableType = 3; // length of the table table type in the table extract
    public const int tableLinesToSkip = 15; // number of Header lines
    // Table extract fields positions and lengths - end

    // Rules extract fields positions and lengths - start
    //public const int charsToSkip = 30; // number of characters on each line to ignore - to cater for extracted data
    public const int charsToSkip = 35; // number of characters on each line to ignore - to cater for extracted data
    public const int rulesLinesToSkip = 4; // number of Header lines
    public const int posRuleUnit = 3; //position of rule unit (this starts from start of rule seperator)
    public const int lenRuleUnit = 8;
    public const int lenRuleSeperator = 3; //length of the rule seperator string
    // Rules extract fields positions and lengths - end

    // Transaction extract fields positions and lengths - start
    public const int posTransactionName = 30; // position of the transaction name in the transaction extract
    public const int lenTransactionName = 8; // length of the transaction name in the transaction extract
    public const int posBuildRule = 38; // position of the build rule name in the transaction extract
    public const int lenBuildRule = 16; // length of the build rule name in the transaction extract
    public const int posDescription = 83; // position of the transaction description in the transaction extract
    public const int lenDescription = 50; // length of the transaction description in the transaction extract
    public const int posPFKey = 62; // position of the PF key name in the transaction extract
    public const int lenPFKey = 5; // length of the PF key name in the transaction extract
    public const int posPFKeyRule = 67; // position of the PF key rule name in the transaction extract
    public const int lenPFKeyRule = 16; // length of the PF key rule name in the transaction extract
    public const int posTransactionUnit = 54; //position of the unit name in the transaction extract
    public const int transactionLinesToSkip = 10; // number of Header lines
    // Transaction rules extract fields positions and lengths - end

    //Triggers extract fields positions and lengths - start
    public const int posTriggerTableName = 18; // position of the Table name in the Trigger extract
    public const int posTriggerRuleName = 34; // position of the Rule name in the Trigger extract
    public const int lenTriggerRuleName = 16; // length of the Rule name in the Trigger extract
    public const int posTriggerAccess = 50; //position of the fire code in the Trigger extract
    public const int triggerLinesToSkip = 4; // number of Header lines
	//Triggers extract fields positions and lengths - end

	  #endregion

	#region excel file extract constants

	//public const int posTableName = 30; // position of the table name in the table extract
	//public const int lenTableName = 16; // length of the table name in the table extract
	//public const int posFieldName = 46; // position of the field name in the table extract

	#endregion

    #region C# List for Oracle packages
    public const int posFunctionName = 9; // position of the function name from the package List
    public const int posProcedureName = 10; // position of the procedure name from the package List
    #endregion

    #region Exceptions
    // List of runtime exceptions defined in ObjectStar
    public static string[] runtimeExceptions = 
    {
      "DELETEFAIL",
      "DISPLAYFAIL",
      "GETFAIL",
      "INSERTFAIL",
      "REPLACEFAIL",
      "COMMITLIMIT",
      "DEFINITIONFAIL",
      "LOCKFAIL",
      "SECURITYFAIL",
      "SERVERBUSY",
      "SERVERERROR",
      "SERVERFAIL",
      "VALIDATEFAIL",
      "FORMLOADFAIL",
      "GUIVALIDATEFAIL",
      "INVALIDATTR",
      "INVALIDATTRVALUE",
      "INVALIDCNTN",
      "INVALIDCTRL",
      "INVALIDENVIR",
      "INVALIDFORM",
      "INVALIDMETHOD",
      "INVALIDPARMVALUE",
      "INVALIDTRANS",
      "INVALIDVAR",
      "METHODFAIL",
      "READONLYATTR",
      "TRANSLOADFAIL",
      "UNSUPPORTEDATTR",
      "CONVERSION",
      "DATAREFERENCE",
      "EXECUTEFAIL",
      "NULLVALUE",
      "OVERFLOW",
      "RANGEERROR",
      "ROUTINEFAIL",
      "STRINGSIZE",
      "UNASSIGNED",
      "UNDERFLOW",
      "ZERODIVIDE"
    };
	#endregion

  }
}
