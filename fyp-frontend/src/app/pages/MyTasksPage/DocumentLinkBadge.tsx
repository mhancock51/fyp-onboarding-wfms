import Api from '@/api';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';
import { Document } from '@/models/Document';
import { Badge } from '@/components/ui/badge';

interface Props {
  documentId: string;
}

export default function DocumentLinkBadge(props: Props) {
  const [document, setDocument] = useState<Document | null>(null);
  const [loaded, setLoaded] = useState<boolean>(false);

  async function fetchDocumentData() {
    await Api.fetchDocumentData(props.documentId) 
    .then((response) => {
      setDocument(response.data.data as Document);
    })
    .catch((error) => {
      toast("Failed to retrieve document data");
    })
    .finally(() => {
      setLoaded(true);
    })
  }

  useEffect(() => {
    void fetchDocumentData();
  }, [props.documentId]);

  return (
    <Badge onClick={() => {void Api.fetchDocument(document?.id ?? "");}} className='cursor-pointer rounded-full p-2 min-w-25'>
      <span>
      {
        document === null && loaded &&
        "ERROR"
      }
      {
        document !== null &&
        document.fileName
      }
      </span>
    </Badge>
  )
}
