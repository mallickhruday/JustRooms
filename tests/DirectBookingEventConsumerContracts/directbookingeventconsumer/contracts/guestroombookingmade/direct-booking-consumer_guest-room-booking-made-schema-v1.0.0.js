const BaseJoi = require('joi');
const Joi = BaseJoi.extend(require('joi-phone-number'));

const moneySchema = Joi.object().keys({
    amount: Joi.number().integer().min(1).max(999).required(),
    currency: Joi.string().min(3).max(3).required()
});

const schema = Joi.object().keys({
    bookingId: Joi.string().guid().required(),
    dateOfFirstNight: Joi.string().isoDate().required(),
    roomType: Joi.string().valid('Single', 'Double', 'Triple', 'Quad', 'Queen', 'King', 'Twin', 'DoubleDouble', 'Studio', 'JuniorSuite', 'MasterSuite').required(),
    price: moneySchema,
    numberOfNights: Joi.number().integer().min(1).max(180).required(),
    numberOfGuests: Joi.number().integer().min(1).max(4).required(),
    firstName: Joi.string().alphanum().min(3).max(30).required(),
    lastName: Joi.string().alphanum().min(3).max(30).required(),
});

//NB: We need to export the schema, so that it can be validated
module.exports.schema = schema;