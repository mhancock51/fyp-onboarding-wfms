import Api from '@/api';
import { useEffect, useState } from 'react'
import { toast } from 'sonner';
import { Document } from '@/models/Document';
import { Badge } from '@/components/ui/badge';
import Utils from '@/util';
import { AxiosError } from 'axios';
import { Download } from 'lucide-react';

interface Props {
  documentId: string;
}

export default function DocumentLinkBadge(props: Props) {
  const [document, setDocument] = useState<Document | null>(null);
  const [loaded, setLoaded] = useState<boolean>(false);

  const [error, setError] = useState<string>("");

  async function fetchDocumentData() {
    await Api.fetchDocumentData(props.documentId) 
    .then((response) => {
      setDocument(response.data.data as Document);
    })
    .catch((error: AxiosError) => {
      if (error.status === 403) {
        setError("Access Forbidden");
        toast.error("Access to document forbidden");
      }
      else {
        setError("Error");
        toast.error("Failed to retrieve document");
      }
    })
    .finally(() => {
      setLoaded(true);
    })
  }

  useEffect(() => {
    void fetchDocumentData();
  }, [props.documentId]);

  return (
    <>
      {
        document === null && loaded &&
        <Badge className='cursor-pointer rounded-full p-2 min-w-25'>
          {error}
        </Badge>
      }
      {
        document !== null && loaded &&
        <Badge className='cursor-pointer rounded-full p-2 min-w-25 flex flex-row justify-center'
          onClick={() => {Utils.downloadFile(document?.documentData, `${document?.fileName}${document?.fileExtension}`)}}
        >
          {document.fileName}
          <Download/>
        </Badge>
      }
    </>
  )
}
