// this is a chunking array utility function that splits an array into chunks of a specified size
Array.prototype.chunk = function (size) {
    if (!Number.isInteger(size) || size <= 0) {
        throw new Error('Size must be a positive integer.');
    }

    const result = [];
    for (let i = 0; i < this.length; i += size) {
        result.push(this.slice(i, i + size));
    }

    return result;
};
