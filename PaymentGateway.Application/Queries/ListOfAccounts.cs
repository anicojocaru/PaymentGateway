using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using FluentValidation;
using static PaymentGateway.Application.Queries.ListOfAccounts.Validator2;

namespace PaymentGateway.Application.Queries
{
    public class ListOfAccounts
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator(Database _database)
            {
                RuleFor(q => q).Must(query =>
                {
                    var person = query.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.Id == query.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.Cnp == query.Cnp);

                    return person != null;
                }).WithMessage("Customer not found");
            }
        }
        public class Validator2 : AbstractValidator<Query>
        {
            public Validator2(Database database)
            {
                RuleFor(q => q).Must(q =>
                {
                    return q.PersonId.HasValue || !string.IsNullOrEmpty(q.Cnp);
                }).WithMessage("Customer data is invalid");

                RuleFor(q => q.Cnp).Must(cnp =>
                {
                    if (string.IsNullOrEmpty(cnp))
                    {
                        return true;
                    }
                    return cnp.Length == 13;
                }).WithMessage("CNP has wrong lenght. Expected 13");

                RuleFor(q => q.PersonId).Must(personId =>
                {
                    if (!personId.HasValue)
                    {
                        return true;
                    }
                    return personId.Value > 0;
                }).WithMessage("Person id is not positive");
            }
        }
        public class Query : IRequest<List<Model>>
        {
            public int? PersonId { get; set; }
            public string Cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly Database _database;
            public QueryHandler(Database database)
            {
                _database = database;
            }

            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                var person = request.PersonId.HasValue ?
                   _database.Persons.FirstOrDefault(x => x.Id == request.PersonId) :
                   _database.Persons.FirstOrDefault(x => x.Cnp == request.Cnp);

                var db = _database.Accounts.Where(x => x.PersonId == person.Id);
                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency = x.Currency,
                    Iban = x.Iban,
                    Id = x.Id,
                    Limit = x.Limit,
                    Status = x.Status,
                    Type = x.Type
                }).ToList();
                return Task.FromResult(result);
            }
        }
        public class Model
        {
            public int Id { get; set; }
            public double Balance { get; set; }
            public string Currency { get; set; }
            public string Iban { get; set; }
            public string Status { get; set; }
            public double Limit { get; set; }
            public string Type { get; set; }
        }
    }
}
