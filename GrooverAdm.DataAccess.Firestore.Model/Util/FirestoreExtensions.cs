using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Firrestore.Util
{
    public static class FirestoreExtensions
    {
        public static async Task DeleteCollection(this CollectionReference collectionReference, int batchSize)
        {
            QuerySnapshot snapshot = await collectionReference.Limit(batchSize).GetSnapshotAsync();
            IReadOnlyList<DocumentSnapshot> documents = snapshot.Documents;
            while (documents.Count > 0)
            {
                foreach (DocumentSnapshot document in documents)
                {
                    Console.WriteLine("Deleting document {0}", document.Id);
                    await document.Reference.DeleteAsync();
                }
                snapshot = await collectionReference.Limit(batchSize).GetSnapshotAsync();
                documents = snapshot.Documents;
            }
            Console.WriteLine("Finished deleting all documents from the collection.");
        }
    }
}
