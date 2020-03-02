// Since Seeding in Mongo is done in alphabetical order... It's is important to keep
// file names alphabetically ordered, if multiple files are to be run.
var denormStateCollection = "EtlDenormState";
var filesBatchCollection = "EtlFilesBatch";
var reportingCollection = "EtlReporting";
var orderCodeCollection = "AIMLOrderCodeRecommendations";
var productVariantCollection = "AIMLProductVariantRecommendations";

db.createCollection(denormStateCollection, function (err, res) {
    if (err) {
	    console.error("Cannot create collection!", denormStateCollection);
		throw err;
	}
    console.log("Collection created!", denormStateCollection);
});

db.createCollection(filesBatchCollection, function (err, res) {
	if (err) {
		console.error("Cannot create collection!", filesBatchCollection);
		throw err;
	}
    console.log("Collection created!", filesBatchCollection);
});

db.createCollection(reportingCollection, function (err, res) {
	if (err) {
        console.error("Cannot create collection!", reportingCollection);
		throw err;
	}
    console.log("Collection created!", reportingCollection);
});

db.createCollection(orderCodeCollection, function (err, res) {
	if (err) {
        console.error("Cannot create collection!", orderCodeCollection);
		throw err;
	}
    console.log("Collection created!", orderCodeCollection);
});

db.createCollection(productVariantCollection, function (err, res) {
	if (err) {
        console.error("Cannot create collection!", productVariantCollection);
		throw err;
	}
    console.log("Collection created!", productVariantCollection);
});
