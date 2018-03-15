// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       Timotheus Pokorra <tp@tbits.net>
//
// Copyright 2017-2018 by TBits.net
//
// This file is part of OpenPetra.
//
// OpenPetra is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// OpenPetra is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with OpenPetra.  If not, see <http://www.gnu.org/licenses/>.
//

class MaintainUsersForm extends JSForm {
	constructor() {
		super();
		super.initEvents();
		super.initSearch(
			'serverMSysMan.asmx/TMaintenanceWebConnector_LoadUsersAndModulePermissions',
			{});
		super.search();
	}

	getMainTableFromResult(result) {
		return result.result.SUser;
	}
	getKeyFromRow(row) {
		return row['s_user_id_c'];
	}
}

$(function() {
	var form = new MaintainUsersForm();
});

