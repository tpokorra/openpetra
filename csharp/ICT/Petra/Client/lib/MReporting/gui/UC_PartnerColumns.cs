/* auto generated with nant generateWinforms from UC_PartnerColumns.yaml and template controlMaintainTable
 *
 * DO NOT edit manually, DO NOT edit with the designer
 *
 */
/*************************************************************************
 *
 * DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * @Authors:
 *       auto generated
 *
 * Copyright 2004-2009 by OM International
 *
 * This file is part of OpenPetra.org.
 *
 * OpenPetra.org is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * OpenPetra.org is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with OpenPetra.org.  If not, see <http://www.gnu.org/licenses/>.
 *
 ************************************************************************/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Ict.Petra.Shared;
using System.Resources;
using System.Collections.Specialized;
using Mono.Unix;
using Ict.Common;
using Ict.Common.Verification;
using Ict.Petra.Client.App.Core;
using Ict.Petra.Client.App.Core.RemoteObjects;
using Ict.Common.Controls;
using Ict.Petra.Client.CommonForms;

namespace Ict.Petra.Client.MReporting.Gui
{

  /// auto generated user control
  public partial class TFrmUC_PartnerColumns: UserControl, Ict.Petra.Client.CommonForms.IFrmPetra
  {
    private TFrmPetraReportingUtils FPetraUtilsObject;

    private Ict.Petra.Shared.MPartner.Partner.Data.PartnerInfoTDS FMainDS;

    /// constructor
    public TFrmUC_PartnerColumns() : base()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();
      #region CATALOGI18N

      // this code has been inserted by GenerateI18N, all changes in this region will be overwritten by GenerateI18N
      this.btnDummy.Text = Catalog.GetString("DummyButton");
      this.btnMoveColumn2Left.Text = Catalog.GetString("Left:");
      this.btnMoveColumn2Right.Text = Catalog.GetString("Right");
      this.btnAddColumn.Text = Catalog.GetString("Add");
      this.btnRemoveColumn.Text = Catalog.GetString("Remove");
      this.lblCalculation.Text = Catalog.GetString("Content of Column::");
      this.lblColumnWidth.Text = Catalog.GetString("Width of Column::");
      this.lblCm.Text = Catalog.GetString("cm:");
      this.btnCancel.Text = Catalog.GetString("&Cancel");
      this.btnApply.Text = Catalog.GetString("A&pply");
      this.grpDefineColumn.Text = Catalog.GetString("Define Column");
      #endregion

    }

    /// helper object for the whole screen
    public TFrmPetraReportingUtils PetraUtilsObject
    {
        set
        {
            FPetraUtilsObject = value;
        }
    }

    /// dataset for the whole screen
    public Ict.Petra.Shared.MPartner.Partner.Data.PartnerInfoTDS MainDS
    {
        set
        {
            FMainDS = value;
        }
    }

    /// needs to be called after FMainDS and FPetraUtilsObject have been set
    public void InitUserControl()
    {
        FPetraUtilsObject.ActionEnablingEvent += ActionEnabledEvent;
    }

#region Implement interface functions
    /// auto generated
    public void RunOnceOnActivation()
    {
    }

    /// <summary>
    /// Adds event handlers for the appropiate onChange event to call a central procedure
    /// </summary>
    public void HookupAllControls()
    {
    }

    /// auto generated
    public void HookupAllInContainer(Control container)
    {
        FPetraUtilsObject.HookupAllInContainer(container);
    }

    /// auto generated
    public bool CanClose()
    {
        return FPetraUtilsObject.CanClose();
    }

    /// auto generated
    public TFrmPetraUtils GetPetraUtilsObject()
    {
        return (TFrmPetraUtils)FPetraUtilsObject;
    }
#endregion

#region Action Handling

    /// auto generated
    public void ActionEnabledEvent(object sender, ActionEventArgs e)
    {
        if (e.ActionName == "actMoveColumn2Left")
        {
            btnMoveColumn2Left.Enabled = e.Enabled;
        }
        if (e.ActionName == "actMoveColumn2Right")
        {
            btnMoveColumn2Right.Enabled = e.Enabled;
        }
        if (e.ActionName == "actAddColumn")
        {
            btnAddColumn.Enabled = e.Enabled;
        }
        if (e.ActionName == "actRemoveColumn")
        {
            btnRemoveColumn.Enabled = e.Enabled;
        }
        if (e.ActionName == "actCancelColumn")
        {
            btnCancel.Enabled = e.Enabled;
        }
        if (e.ActionName == "actApplyColumn")
        {
            btnApply.Enabled = e.Enabled;
        }
    }

#endregion
  }
}
