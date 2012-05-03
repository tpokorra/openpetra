﻿//
// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       christiank
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
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;
using GNU.Gettext;
using Ict.Common.Verification;
using Ict.Common;
using Ict.Common.IO;
using Ict.Petra.Client.App.Core.RemoteObjects;
using Ict.Petra.Shared.MPersonnel;
using Ict.Petra.Shared.MPersonnel.Personnel.Data;
using Ict.Petra.Shared.MCommon.Validation;

namespace Ict.Petra.Client.MPersonnel.Gui.Setup
{
    public partial class TFrmApplicationFormSetup
    {
        private void NewRowManual(ref PtAppFormTypesRow ARow)
        {
            // Deal with primary key.  FormName is primary key
            string newName = Catalog.GetString("NEWCODE");
            Int32 countNewDetail = 0;

            if (FMainDS.PtAppFormTypes.Rows.Find(new object[] { newName }) != null)
            {
                while (FMainDS.PtAppFormTypes.Rows.Find(new object[] { newName + countNewDetail.ToString() }) != null)
                {
                    countNewDetail++;
                }

                newName += countNewDetail.ToString();
            }

            ARow.FormName = newName;

            ARow.AppUsedBy = "Both";
        }

        private void NewRecord(Object sender, EventArgs e)
        {
            CreateNewPtAppFormTypes();
        }

        private void ValidateDataDetailsManual(PtAppFormTypesRow ARow)
        {
            DataColumn ValidationColumn = ARow.Table.Columns[PtAppFormTypesTable.ColumnFormSentIndicatorId];
            TVerificationResultCollection VerificationResultCollection = FPetraUtilsObject.VerificationResultCollection;

            TVerificationResult VerificationResult = TGuiChecks.ValidateCheckBoxPairIsChecked(chkDetailFormSentIndicator, chkDetailFormReceivedIndicator);
            if (VerificationResult != null)
            {
                VerificationResult = new TScreenVerificationResult(this, ValidationColumn, VerificationResult.ResultText, Catalog.GetString("Sending/Receiving"), 
                    VerificationResult.ResultCode, chkDetailFormSentIndicator, TResultSeverity.Resv_Critical);
            }

            // Handle addition to/removal from TVerificationResultCollection
            VerificationResultCollection.Auto_Add_Or_AddOrRemove(this, VerificationResult, ValidationColumn);
        }
    }
}