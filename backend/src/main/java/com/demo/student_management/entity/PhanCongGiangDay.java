package com.demo.student_management.entity;

import jakarta.persistence.*;
import lombok.*;

@Entity
@Table(name = "PHANCONG_GIANGDAY")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class PhanCongGiangDay {

    @Id
    @Column(name = "ID_PHANCONG", columnDefinition = "char(36)", nullable = false)
    @EqualsAndHashCode.Include
    private String idPhanCong;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_GIAOVIEN", columnDefinition = "varchar(10)", nullable = false)
    private GiaoVien giaoVien;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_LOP", columnDefinition = "char(5)", nullable = false)
    private Lop lop;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_MONHOC", columnDefinition = "varchar(20)", nullable = false)
    private MonHoc monHoc;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_HOCKY", columnDefinition = "varchar(20)", nullable = false)
    private HocKy hocKy;
}
