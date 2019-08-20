// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       Timotheus Pokorra <tp@tbits.net>
//
// Copyright 2017-2018 by TBits.net
// Copyright 2019 by SolidCharity.com
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

'use strict';

const e = React.createElement;

const columns = [
  { key: "id", name: "ID", editable: true },
  { key: "title", name: "Title", editable: true },
  { key: "complete", name: "Complete", editable: true }
];

const rows = [
  { id: 0, title: "Task 1", complete: 20 },
  { id: 1, title: "Task 2", complete: 40 },
  { id: 2, title: "Task 3", complete: 60 }
];

class BatchGrid extends React.Component {
  constructor(props) {
    super(props);
    this.state = { rows };
  }
/*
  onGridRowsUpdated = ({ fromRow, toRow, updated }) => {
    this.setState(state => {
      const rows = state.rows.slice();
      for (let i = fromRow; i <= toRow; i++) {
        rows[i] = { ...rows[i], ...updated };
      }
      return { rows };
    });
  };
  */
  render() {
    return React.createElement("ReactDataGrid", {
        columns:columns,
        rowGetter:function(i){return this.state.rows[i];},
        rowsCount:3,
        onGridRowsUpdated: this.onGridRowsUpdated,
        enableCellSelect:true});
  }
}

const domContainer = document.querySelector('#like_button_container');
ReactDOM.render(e(BatchGrid), domContainer);


$('document').ready(function () {
	get_available_years(
		'#tabfilter [name=ABatchYear]',
		'#tabfilter [name=ABatchPeriod]',
		'',
		function() {
			load_preset();
		});
});

function updatePeriods(year) {
	get_available_periods(year, '#tabfilter [name=ABatchPeriod]', display_list, true);
}

function load_preset() {
	var x = window.localStorage.getItem('GLBatches');
	if (x != null) {
		x = JSON.parse(x);
		// first set the period so that it can be used when the year has been selected
		let y = {"ABatchPeriod":x["ABatchPeriod"]};
		format_tpl($('#tabfilter'), y);
		format_tpl($('#tabfilter'), x);
		display_list();
	}
}

function display_list(source, BatchNumber) {
	if (source == null) {
		source = "filter";
	}
	// x is search
	if (source == "filter") {
		var x = extract_data( $('#tabfilter') );
	} else if (source == "preset") {
		var x = window.localStorage.getItem('GLBatches');
		x = JSON.parse(x);
		format_tpl( $('#tabfilter'), x );
	}
	x['ALedgerNumber'] = window.localStorage.getItem('current_ledger');

	api.post('serverMFinance.asmx/TGLTransactionWebConnector_LoadABatch', x).then(function (data) {
		data = JSON.parse(data.data.d);
		// on reload, clear content
		$('#browse_container').html('');
		for (item of data.result.ABatch) {
			format_item(item);
		}
		format_currency(data.ACurrencyCode);
		format_date();

		if (BatchNumber != null) {
			open_transactions($(), BatchNumber);
		}
	})
}

function updateBatch(BatchNumber) {
	alert(BatchNumber);

	var x = {
		'ALedgerNumber': window.localStorage.getItem('current_ledger'),
		'ABatchNumber': BatchNumber};

	api.post('serverMFinance.asmx/TGLTransactionWebConnector_LoadABatch2', x).then(function (data) {
		data = JSON.parse(data.data.d);
		item = data.result.ABatch[0];
		let row = format_tpl($("[phantom] .tpl_row").clone(), item);
		$('#Batch' + BatchNumber + " div").first().html(row.children()[0]);
	});
}

function update_list(source) {
	console.log("update_list");
}

function format_date() {
	$('.format_date').each(
		function(x, obj) {
			obj = $(obj);
			let t = /\((.+)\)/g.exec(obj.text());
			if (t == null || t.length <=1) {return}

			time = new Date(parseInt(t[1])).toLocaleDateString();
			obj.text(time);

		}
	)
};

function format_item(item) {
	let row = format_tpl($("[phantom] .tpl_row").clone(), item);
	// let view = format_tpl($("[phantom] .tpl_view").clone(), item);
	// row.find('.collapse_col').append(view);
	$('#browse_container').append(row);
}

function open_transactions(obj, number) {
	obj = $(obj);
	if (obj.find('.collapse').is(':visible') ) {
		return;
	}
	if (obj.find('[batch-status]').text() == "Posted" ) {
		obj.find('.only_show_when_not_posted').hide();
	}
	else {
		obj.find('.only_show_when_posted').hide();
	}
	let x = {"ALedgerNumber":window.localStorage.getItem('current_ledger'), "ABatchNumber":number};
	api.post('serverMFinance.asmx/TGLTransactionWebConnector_LoadABatchAJournalATransaction', x).then(function (data) {
		data = JSON.parse(data.data.d);
		// on open, clear content

		let place_to_put_content = obj.find('.content_col').html('');
		for (item of data.result.ATransaction) {
			if (item['a_debit_credit_indicator_l']) {
				item['debitcredit'] = i18next.t('GLBatches.DEBIT');
				item['creditamountbase'] = '';
				item['debitamountbase'] = item['a_amount_in_base_currency_n'];
			} else {
				item['debitcredit'] = i18next.t('GLBatches.CREDIT');
				item['debitamountbase'] = '';
				item['creditamountbase'] = item['a_amount_in_base_currency_n'];
			}
			// console.log(item);
			let transaction_row = $('[phantom] .tpl_transaction').clone();
			transaction_row = format_tpl(transaction_row, item);
			place_to_put_content.append(transaction_row);
		}
		format_currency(data.ACurrencyCode);
		format_date();
		$('.tpl_row .collapse').collapse('hide');
		obj.find('.collapse').collapse('show')
	})

}

/////

var new_entry_data = {};
function new_batch() {
	let x = {ALedgerNumber :window.localStorage.getItem('current_ledger')};
	api.post('serverMFinance.asmx/TGLTransactionWebConnector_CreateABatch', x).then(
		function (data) {
			parsed = JSON.parse(data.data.d);
			new_entry_data = parsed.result;
			let p = format_tpl( $('[phantom] .tpl_edit_batch').clone(), parsed['result']['ABatch'][0] );
			$('#modal_space').html(p);
			p.find('input[name=a_batch_credit_total_n]').attr('readonly', false);
			p.find('input[name=a_batch_debit_total_n]').attr('readonly', false);
			p.find('[action]').val('create');
			p.modal('show');
		}
	)
};

function new_trans(batch_number, batch_date) {
	ledger_number = window.localStorage.getItem('current_ledger');
	new_entry_data = [];
	new_entry_data['a_ledger_number_i'] = ledger_number;
	new_entry_data['a_batch_number_i'] = batch_number;
	new_entry_data['a_transaction_number_i'] = $("#Batch" + batch_number + " .tpl_transaction").length + 1;
	new_entry_data['a_account_code_c'] = "0100";
	new_entry_data['a_cost_centre_code_c'] = ledger_number * 100;
	new_entry_data['a_transaction_date_d'] = batch_date;
	let p = format_tpl( $('[phantom] .tpl_edit_trans').clone(), new_entry_data );
	$('#modal_space').html(p);
	p.find('[action]').val('create');
	p.modal('show');
};

/////

function edit_batch(batch_id) {
	var x = window.localStorage.getItem('GLBatches');
	if (x == null) {
		x = extract_data( $('#tabfilter') );
	} else {
		x = JSON.parse(x);
	}
	x['ALedgerNumber'] = window.localStorage.getItem('current_ledger');
	// on open of a edit modal, we get new data,
	// so everything is up to date and we don't have to load it, if we only search
	api.post('serverMFinance.asmx/TGLTransactionWebConnector_LoadABatch', x).then(function (data) {
		parsed = JSON.parse(data.data.d);
		let searched = null;
		new_entry_data = parsed.result;
		for (batch of parsed.result.ABatch) {
			if (batch.a_batch_number_i == batch_id) {
				searched = batch;
				break;
			}
		}
		if (searched == null) {
			return alert('ERROR');
		}
		let tpl_m = format_tpl( $('[phantom] .tpl_edit_batch').clone(), searched );
		if (searched['a_batch_status_c'] == "Posted") {
			tpl_m.find('.posted_readonly').attr('readonly', true)
		}
		$('#modal_space').html(tpl_m);
		tpl_m.find('[action]').val('edit');
		tpl_m.modal('show');

	})
}

function edit_trans(batch_id, trans_id) {
	let x = {"ALedgerNumber":window.localStorage.getItem('current_ledger'), "ABatchNumber":batch_id};
	// on open of a edit modal, we get new data,
	// so everything is up to date and we don't have to load it, if we only search
	api.post('serverMFinance.asmx/TGLTransactionWebConnector_LoadABatchAJournalATransaction', x).then(function (data) {
		parsed = JSON.parse(data.data.d);
		let searched = null;
		new_entry_data = parsed.result;
		for (trans of parsed.result.ATransaction) {
			if (trans.a_transaction_number_i == trans_id) {
				searched = trans;
				break;
			}
		}
		if (searched == null) {
			return alert('ERROR');
		}
		if (searched['a_debit_credit_indicator_l']) {
			searched['a_debit_amount_base_n'] = searched['a_amount_in_base_currency_n'];
			searched['a_credit_amount_base_n'] = 0.0;
		} else {
			searched['a_debit_amount_base_n'] = 0.0;
			searched['a_credit_amount_base_n'] = searched['a_amount_in_base_currency_n'];
		}
		searched['a_account_name_c'] = searched['a_account_code_c'];
		searched['a_cost_center_name_c'] = searched['a_cost_centre_code_c'];
		let tpl_m = format_tpl( $('[phantom] .tpl_edit_trans').clone(), searched );
		if (searched['a_batch_status_c'] == "Posted") {
			tpl_m.find('.posted_readonly').attr('readonly', true)
		}
		$('#modal_space').html(tpl_m);
		tpl_m.find('[action]').val('edit');
		tpl_m.modal('show');

	})
}

/////

function save_edit_batch(obj_modal) {
	let obj = $(obj_modal).closest('.modal');
	let mode = obj.find('[action]').val();

	// extract information from a jquery object
	let payload = translate_to_server( extract_data(obj) );
	payload['action'] = mode;

	api.post('serverMFinance.asmx/TGLTransactionWebConnector_MaintainBatches', payload).then(function (result) {
		parsed = JSON.parse(result.data.d);
		if (parsed.result == true) {
			display_message(i18next.t('forms.saved'), "success");
			$('#modal_space .modal').modal('hide');
			updateBatch(payload['ABatchNumber']);
		}
		if (parsed.result == "false") {
			for (msg of parsed.AVerificationResult) {
				display_message(i18next.t(msg.code), "fail");
			}
		}
	});
}

function save_edit_trans(obj_modal) {
	let obj = $(obj_modal).closest('.modal');
	let mode = obj.find('[action]').val();

	// extract information from a jquery object
	let payload = translate_to_server( extract_data(obj) );
 	payload['action'] = mode;
 	payload['AJournalNumber'] = 1;
	payload['AAmountInIntlCurrency'] = 0.0;
	let amount = payload['ACreditAmountBase'] - payload['ADebitAmountBase'];
	if (amount < 0) {
		payload['AAmountInBaseCurrency'] = -1 * amount;
		payload['ADebitCreditIndicator'] = true;
	} else {
		payload['AAmountInBaseCurrency'] = amount;
		payload['ADebitCreditIndicator'] = false;
	}
	api.post('serverMFinance.asmx/TGLTransactionWebConnector_MaintainTransactions', payload).then(function (result) {
		parsed = JSON.parse(result.data.d);
		if (parsed.result == true) {
			display_message(i18next.t('forms.saved'), "success");
			$('#modal_space .modal').modal('hide');
			update_list();
		}
		if (parsed.result == "false") {
			for (msg of parsed.AVerificationResult) {
				display_message(i18next.t(msg.code), "fail");
			}
		}

	});

}

function delete_edit_trans(obj_modal) {
	let obj = $(obj_modal).closest('.modal');

	// extract information from a jquery object
	let payload = translate_to_server( extract_data(obj) );
	payload['action'] = "delete";
	payload['AJournalNumber'] = 1;
	payload['AAmountInIntlCurrency'] = 0.0;
	payload['AAmountInBaseCurrency'] = 0.0;
	payload['ADebitCreditIndicator'] = true;
	api.post('serverMFinance.asmx/TGLTransactionWebConnector_MaintainTransactions', payload).then(function (result) {
		parsed = JSON.parse(result.data.d);
		if (parsed.result == true) {
			display_message(i18next.t('forms.deleted'), "success");
			$('#modal_space .modal').modal('hide');
			display_list();
		}
		if (parsed.result == "false") {
			for (msg of parsed.AVerificationResult) {
				display_message(i18next.t(msg.code), "fail");
			}
		}

	});
}

function importTransactions(batch_id, csv_file) {
	if (csv_file == undefined) {
		$('#Batch'+batch_id).find('.import_space').css('display', 'block');
		return;
	}

	let x = {
		AImportString: csv_file,
		ALedgerNumber: window.localStorage.getItem('current_ledger'),
		ABatchNumber: batch_id,
		AJournalNumber: 1,
		NumberFormat: "European",
		DateFormatString: "dmy"
	};

	api.post('serverMFinance.asmx/TGLTransactionWebConnector_ImportGLTransactions', x).then(function (result) {
		parsed = JSON.parse(result.data.d);
		if (parsed.result == true) {
			display_message(i18next.t('forms.saved'), "success");
			display_list();
		}
		if (parsed.result == "false") {
			for (msg of parsed.AVerificationResult) {
				display_message(i18next.t(msg.code), "fail");
			}
		}
	})
}


/////

function test_post(batch_id) {
	let x = {
		ALedgerNumber: window.localStorage.getItem('current_ledger'),
		ABatchNumber: batch_id
	};
	api.post( 'serverMFinance.asmx/TGLTransactionWebConnector_TestPostGLBatch', x).then(function (data) {
		data = JSON.parse(data.data.d);
		if (data.result == true) {
			display_message ( data.ResultingBalances );
		} else {
			display_error( data.AVerifications );
		}
	})
}

function batch_post(batch_id) {
	let x = {
		ALedgerNumber: window.localStorage.getItem('current_ledger'),
		ABatchNumber: batch_id
	};
	api.post( 'serverMFinance.asmx/TGLTransactionWebConnector_PostGLBatch', x).then(function (data) {
		data = JSON.parse(data.data.d);
		if (data.result == true) {
			display_message( i18next.t('GLBatches.success_posted'), 'success' );
			display_list('filter');
		} else {
			display_error( data.AVerifications );
		}
	})
}

function batch_cancel(batch_id) {
	let x = {
		ALedgerNumber: window.localStorage.getItem('current_ledger'),
		ABatchNumber: batch_id
	};
	api.post( 'serverMFinance.asmx/TGLTransactionWebConnector_CancelGLBatch', x).then(function (data) {
		data = JSON.parse(data.data.d);
		if (data.result == true) {
			display_message( i18next.t('GLBatches.success_cancelled'), 'success' );
			display_list('filter');
		} else {
			display_error( data.AVerificationResult );
		}
	})
}

function batch_reverse(batch_id, date) {
	var date_extractor = new RegExp(/Date\((.+)\)/g);
	var date = new Date( parseInt( date_extractor.exec(date)[1] ) );
	var date_str = date.getFullYear()+"-"+(date.getMonth()+1)+"-"+date.getDate();
	let x = {
		ALedgerNumber: window.localStorage.getItem('current_ledger'),
		ABatchNumberToReverse: batch_id,
		// TODO use reasonable date for reversal???
		ADateForReversal: "2018-07-01",//date_str,
		AAutoPostReverseBatch: false
	};
	api.post( 'serverMFinance.asmx/TGLTransactionWebConnector_ReverseBatch', x).then(function (data) {
		data = JSON.parse(data.data.d);
		console.log(data);
	})
}
