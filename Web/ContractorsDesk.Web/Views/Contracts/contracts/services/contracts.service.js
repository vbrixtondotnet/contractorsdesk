class ContractsService {
    constructor() {
        this.httpService = new httpService();
    }
}

class ContractsCKEditorInserImageAdapter {
	constructor(loader) {
		this.loader = loader;
	}

	upload() {
		return this.loader.file.then(file => {
			return new Promise((resolve, reject) => {
				const reader = new FileReader();
				reader.onload = () => {
					resolve({ default: reader.result });
				};
				reader.onerror = err => reject(err);
				reader.readAsDataURL(file);
			});
		});
	}
}

function debouncedManualSave(fn, wait = 1000) {
	let t;
	return (...args) => {
		clearTimeout(t);
		t = setTimeout(() => fn.apply(null, args), wait);
	};
}