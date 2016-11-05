var EntitiesViewBaseModel = function (entityName, entityModelCreator, columns)
{
    var self = this;

    var getEntityName = function ()
    {
        if (entityName[entityName.length - 1] == 'y')
            return entityName.substr(0, entityName.length - 1) + "ies";
        return entityName + "s";
    };

    self.grid = new CustomGrid({ columns: columns, pager: { pageSize: 10 } });
    self.grid.onApplyChanges.subscribe(function (model)
    {
        window.AdminDataProvider["Save" + entityName](
            CustomGrid.mapToEntity(model),
            function (id)
            {
                model.Id.value(id);
            }
        );
    });
    self.grid.onApplyCancel.subscribe(function (model) { if (model.Id.value() == 0) self.grid.removeRow(model); });
    self.grid.onDeleteRow.subscribe(function (model)
    {
        window.AdminDataProvider["Delete" + entityName](
            model.Id.value(),
            function () { },
            function ()
            {
                alert('error');
            }
        );
    });

    self["add" + entityName] = function ()
    {
        self.grid.addRow(CustomGrid.mapToModel(entityModelCreator()));
        self.grid.pager.page(self.grid.pager.pagesCount());
    };

    return self;
};