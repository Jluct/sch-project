var CustomGrid = function (config)
{
    var self = this;
    var _rows = ko.observableArray([]);
    var _rowsTrigger = ko.observable();

    self.columns = ko.observableArray($.extend([], config.columns));
    config.pager = config.pager || {};
    self.pager = {
        page: ko.observable(ko.toJS(config.pager.page) || 1),
        pageSize: ko.observable(ko.toJS(config.pager.pageSize) || Number.MAX_VALUE),
        pagesCount: ko.computed(function ()
        {
            _rowsTrigger();
            if (!self.pager)
                return 1;
            var page = Math.round(_rows().length / self.pager.pageSize()) || 1;
            if (self.pager.page() > page)
                self.pager.page(page);
            return page;
        })
    };
    self.rows = ko.computed({
        read: function ()
        {
            _rowsTrigger();
            var startIndex = (self.pager.page() - 1) * self.pager.pageSize();
            return _rows().slice(startIndex, startIndex + self.pager.pageSize());
        },
        write: function (array)
        {
            _rows(array);
            self.selectedRow(null);
            _rowsTrigger.valueHasMutated();
        }
    });

    self.editingItem = ko.observable();
    self.selectedRow = ko.observable();

    self.onApplyCancel = new Event();
    self.onApplyChanges = new Event();
    self.onAddRow = new Event();
    self.onDeleteRow = new Event();
    self.onBeforeDeleteRow = new Event();
    self.onBeginEditRow = new Event();

    //  create the transaction for commit and reject
    self.editTransaction = new ko.subscribable();

    //  helpers
    self.isItemEditing = function (row)
    {
        return row == self.editingItem();
    };

    self.changePage = function (page)
    {
        if (0 < page && page <= self.pager.pagesCount())
            self.pager.page(page);
    };

    self.setRows = function (rows)
    {
    };

    self.addRow = function (row)
    {
        if (self.editingItem() != null)
            return;

        _rows.push(row);

        //  begin editing the new item straight away
        self.editRow(row);
        self.selectedRow(row);
    };

    //  behaviour
    self.removeRow = function (row)
    {
        if (self.editingItem() == null)
        {
            var res = self.onBeforeDeleteRow.invoke(row);
            if (res.length == 0 || res.every(function (element, index, array) { return element; }))
            {
                _rows.remove(row);
                if (self.selectedRow() == row)
                    self.selectedRow(null);
                _rowsTrigger.valueHasMutated();
                self.onDeleteRow.invoke(row);
            }
        }
    };

    self.editRow = function (row)
    {
        if (self.editingItem() == null)
        {
            // start the transaction
            //row.beginEdit(self.editTransaction);
            for (var field in row)
                if (row[field].value !== undefined && ko.isObservable(row[field].value))
                    row[field].value.beginEdit(self.editTransaction);

            // shows the edit fields
            self.editingItem(row);

            self.onBeginEditRow.invoke(row);
        }
    };

    self.applyRow = function (row)
    {
        //  commit the edit transaction
        self.editTransaction.notifySubscribers(null, "commit");

        //  hides the edit fields
        self.editingItem(null);

        self.onApplyChanges.invoke(row);
    };

    self.cancelEdit = function (row)
    {
        //  reject the edit transaction
        self.editTransaction.notifySubscribers(null, "rollback");

        //  hides the edit fields
        self.editingItem(null);

        self.onApplyCancel.invoke(row);
    };

    return self;
};

CustomGrid.columnTypes = {
    list: 1,
    string: 2,
    boolean: 3,
    html: 4,
    date: 5,
    number: 6
};

CustomGrid.mapToModel = function (row, columns)
{
    row = $.extend({}, row);
    var res = {};
    if (columns)
    {
        for (var index in columns)
            if (row[columns[index].field] == undefined)
                row[columns[index].field] = columns[index].defaultValue;
    }
    for (var i in row)
    {
        if (ko.isObservable(res[i]))
            res[i] = { value: res[i] };
        else
            res[i] = { value: ko.observable(row[i]) };
    }
    return res;
};

CustomGrid.mapToModels = function (rows, columns, eachMapperCallback)
{
    var res = [];
    for (var index in rows)
    {
        var model = CustomGrid.mapToModel(rows[index], columns);
        if (typeof(eachMapperCallback) == "function")
            eachMapperCallback(model, rows[index]);
        res.push(model);
    }
    return res;
};

CustomGrid.mapToEntity = function (row)
{
    var res = { };
    for (var index in row)
    {
        res[index] = ko.toJS(row[index].value);
    }
    return res;
};


/*----------------------------------------------------------------------*/
/* Observable Extention for Editing
/*----------------------------------------------------------------------*/
ko.observable.fn.beginEdit = function (transaction)
{

    var self = this;
    var commitSubscription,
        rollbackSubscription;

    // get the current value and store it for editing
    if (self.slice)
        self.value = ko.observableArray(self.slice());
    else
        self.value = ko.observable(ko.toJS(self));

    self.dispose = function ()
    {
        // kill this subscriptions
        commitSubscription.dispose();
        rollbackSubscription.dispose();
    };

    self.commit = function ()
    {
        // update the actual value with the edit value
        self.value(self());

        // dispose the subscriptions
        self.dispose();
    };

    self.rollback = function ()
    {
        // rollback the edit value
        self(self.value());

        // dispose the subscriptions
        self.dispose();
    };

    //  subscribe to the transation commit and reject calls
    commitSubscription = transaction.subscribe(self.commit,
                                                self,
                                                "commit");

    rollbackSubscription = transaction.subscribe(self.rollback,
                                                    self,
                                                    "rollback");

    return self;
}
