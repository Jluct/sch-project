window.ResourceManager = new function ()
{
    this.getResources = function (value)
    {
        if (value)
        {
            if (typeof (value) == "string")
                return JSON.parse(value);
            else
                return value;
        }
        else
            throw 'Expected json string or object';
    };
};
