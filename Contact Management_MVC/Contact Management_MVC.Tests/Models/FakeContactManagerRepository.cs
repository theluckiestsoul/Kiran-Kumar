﻿using Contact_Management_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contact_Management_MVC.Tests.Models
{
    class FakeContactManagerRepository : IContactManagerRepositiory
    {

        private IList<Group> _groups = new List<Group>();

        #region IContactManagerRepository Members

        // Group methods

        public Group CreateGroup(Group groupToCreate)
        {
            _groups.Add(groupToCreate);
            return groupToCreate;
        }

        public IEnumerable<Group> ListGroups()
        {
            return _groups;
        }

        // Contact methods

        public Contact CreateContact(Contact contactToCreate)
        {
            throw new NotImplementedException();
        }

        public void DeleteContact(Contact contactToDelete)
        {
            throw new NotImplementedException();
        }

        public Contact EditContact(Contact contactToEdit)
        {
            throw new NotImplementedException();
        }

        public Contact GetContact(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Contact> ListContacts()
        {
            throw new NotImplementedException();
        }
        #endregion



        int IContactManagerRepositiory.DeleteContact(Contact Contact)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Contact> GetContactList()
        {
            throw new NotImplementedException();
        }
    }
}

