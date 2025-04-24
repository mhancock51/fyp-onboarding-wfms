import { useEffect } from 'react'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '../ui/table'
import DocumentDTO from '@/models/DTOs/DocumentDTO'
import NoResults from '../NoResults';
import { Spinner } from '../ui/spinner';
import { Label } from '../ui/label';
import { Badge } from '../ui/badge';
import TableActionsDropdown from '../TableActionsDropdown';
import util from '@/util';
import AccountDirectoryBadge from '../AccountDirectoryBadge';

interface Props {
  documents: DocumentDTO[];
  loading: boolean;
  loaded: boolean;
}

export default function DocumentsTable(props: Props) {
  useEffect(() => {
    console.log(props.documents);
  }, [props.documents]);

  return (
    <>
      {
        props.loading && !props.loaded &&
        <div className='flex flex-row gap-2 items-center justify-center'>
          <Spinner/>
          <Label>Loading documents...</Label>
        </div>
      }
      {
        props.loaded && !props.loading && props.documents.length === 0 &&
        <NoResults text={'No documents have been uploaded yet'}/>
      }
      {
        props.loaded && !props.loading && props.documents.length > 0 &&
        <Table className='table-auto w-full'>
          <TableHeader className='justify-start'>
            <TableCell className='text-center' width={200}>File Name</TableCell>
            <TableCell className='text-center' width={200}>Task Name</TableCell>
            <TableCell className='text-center' width={25}>Creator</TableCell>
            <TableCell className='text-center' width={25}>Extension</TableCell>
            <TableCell width={25}>Upload Timestamp</TableCell>
            <TableCell></TableCell>
          </TableHeader>
          <TableBody>
            {
              props.documents.map((document, index) => (
                <TableRow key={index}>
                  <TableCell>{document.fileName}</TableCell>
                  <TableCell>
                    <Badge className='p-2 rounded-full w-full'>
                      {document.taskInstance?.template.name}
                    </Badge>
                  </TableCell>
                  <TableCell>                    
                    <AccountDirectoryBadge accountDirectory={document.creatorsAccount}/>                     
                  </TableCell>
                  <TableCell>
                    <Badge className='p-2 rounded-full w-full'>
                      {document.fileExtension}
                    </Badge>
                  </TableCell>
                  <TableCell>{new Date(document.uploadTimestamp).toLocaleString()}</TableCell>
                  <TableCell>
                    <TableActionsDropdown actions={[
                      {
                        label: "Download document",
                        onClick: () => {util.downloadFile(document.documentData, `${document.fileName}${document.fileExtension}`)}
                      }
                    ]}/>
                  </TableCell>
                </TableRow>
              ))
            }
          </TableBody>
        </Table>
      }
    </>
  )
}
