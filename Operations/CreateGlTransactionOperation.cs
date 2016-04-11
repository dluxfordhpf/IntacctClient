﻿using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Intacct.Entities;

namespace Intacct.Operations
{
    /// <summary>
    /// Creates an journal transaction - a group of journal entries.
    /// </summary>
    /// <remarks>
    /// This operation creates a journal transaction in ANY journal except statistics.  The name is purposefully specific to match the name in the Intacct XML API.
    /// 
    /// USAGE:
    /// 1.  Create a list of at least two IntacctGeneralLedgerEntry objects.
    /// 2.  Create a new IntacctGeneralLedgerTransaction object and add the IntacctGeneralLedgerEntry.  The Transaction must have at least two entry
    ///     objects where the sum of all credits equals the sum of all debits.  Since this is often matched pairs of entries, the AddEntryPair method
    ///     may be useful.
    ///         var newTransaction = new IntacctGeneralLedgerTransaction(IntacctGeneralLedgerTransaction.JournalId_GeneralJournal, "Test of IT Intacct client", null);
    ///         newTransaction.AddEntryPair(100.00M, "AccountNumber", "Note for line items", "SourceClass", "SourceCostCenter", "TargetClass", "TargetCostCenter");
    /// 3.  Create a CreateGlTransactionOperation for the transaction object.  This is typically combined with step 4.
    /// 4.  Execute the operation.
    ///         var response = client.ExecuteOperations(new[] { new CreateGlTransactionOperation(session, newTransaction) }, CancellationToken.None).Result;
    /// 5.  Check the response.  A success is indicated when response.Success && ((OperationResults[0].Errors == null) || (OperationResults[0].Errors.Count == 0)).
    /// 
    /// Notes:
    /// 1.  The create API will fail with PL05000053 if entries do not sum to 0 (balanced debit and credit records).
    /// 2.  Required fields are amount, trtype, datecreated, glaccountno, classid, and costCenter.
    /// </remarks>
    public class CreateGlTransactionOperation : IntacctAuthenticatedOperationBase<IntacctGeneralLedgerTransaction>
    {
        #region Constructor
        public CreateGlTransactionOperation( IIntacctSession newParentSession, IntacctGeneralLedgerTransaction newTransaction, string newReferenceno = null, string newSourceentity = null, IntacctCustomField[] newCustomfields = null, IntacctDate newReverseDate = null )
            : base(newParentSession, "create_gltransaction", "key")
        {
            // Validate parameters
            if (newParentSession == null)
                throw new ArgumentNullException("parentSession");
            if (newTransaction == null)
                throw new ArgumentNullException("newGlTransaction");

            transaction     = newTransaction;
            reverseDate     = newReverseDate;
            referenceno     = newReferenceno;
            sourceentity    = newSourceentity;
            customfields    = newCustomfields;
        }
        #endregion Constructor

        #region Protected IntacctAuthenticatedOperationBase
        protected override XObject[] CreateFunctionContents()
        {
            var serializedElements = new List<XObject>();
            serializedElements.AddRange(transaction.ToXmlElements());
            Util.SerializeChildIntacctObject(reverseDate, "reversedate", serializedElements);
            Util.SerializeStringToXml(referenceno, "referenceno", serializedElements);
            Util.SerializeStringToXml(sourceentity, "sourceentity", serializedElements);
            Util.SerializeArrayOfChildIntacctObject(customfields, "customfields", "customfield", serializedElements);
            return serializedElements.ToArray();
        }

        protected override IntacctOperationResult<IntacctGeneralLedgerTransaction> ProcessResponseData( XElement responseData )
        {
            var deserializedTransaction = new IntacctGeneralLedgerTransaction(responseData);
            return new IntacctOperationResult<IntacctGeneralLedgerTransaction>(deserializedTransaction);
        }
        #endregion Protected IntacctAuthenticatedOperationBase

        #region Private Data Members
        private readonly IntacctGeneralLedgerTransaction    transaction;
        private readonly IntacctDate                        reverseDate;
        private readonly string                             referenceno;
        private readonly string                             sourceentity;
        private readonly IntacctCustomField[]               customfields;
        #endregion Private Data Members
    }
}