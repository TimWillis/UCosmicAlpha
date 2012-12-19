﻿namespace UCosmic.Domain.Establishments
{
    public class UpdateSamlSignOnInfo
    {
        public Establishment Establishment { get; set; }
        public string EntityId { get; set; }
        public string MetadataUrl { get; set; }
    }

    public class HandleUpdateSamlSignOnInfoCommand : IHandleCommands<UpdateSamlSignOnInfo>
    {
        private readonly IHandleCommands<UpdateSamlSignOnMetadata> _samlMetadataHandler;
        private readonly IUnitOfWork _unitOfWork;

        public HandleUpdateSamlSignOnInfoCommand(IHandleCommands<UpdateSamlSignOnMetadata> samlMetadataHandler
            , IUnitOfWork unitOfWork)
        {
            _samlMetadataHandler = samlMetadataHandler;
            _unitOfWork = unitOfWork;
        }

        public void Handle(UpdateSamlSignOnInfo command)
        {
            // when no SAML sign on exists, create one
            if (!command.Establishment.HasSamlSignOn())
            {
                command.Establishment.SamlSignOn = new EstablishmentSamlSignOn
                {
                    EntityId = command.EntityId,
                    MetadataUrl = command.MetadataUrl,
                };
            }

            // when EntityId or MetadataUrl have changed, reset for url/xml update
            else if (command.Establishment.SamlSignOn.EntityId != command.EntityId
                && command.Establishment.SamlSignOn.MetadataUrl != command.MetadataUrl)
            {
                command.Establishment.SamlSignOn.EntityId = command.EntityId;
                command.Establishment.SamlSignOn.MetadataUrl = command.MetadataUrl;
                command.Establishment.SamlSignOn.MetadataXml = null;
                command.Establishment.SamlSignOn.SsoLocation = null;
                command.Establishment.SamlSignOn.SsoBinding = null;
            }

            _unitOfWork.SaveChanges();

            // finally, update the xml using metadata url
            _samlMetadataHandler.Handle(
                new UpdateSamlSignOnMetadata
                {
                    EstablishmentId = command.Establishment.RevisionId,
                }
            );
        }
    }
}
